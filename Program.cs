using System;
using System.Collections.Generic;
using System.Threading;

class BankAccount
{
    public int ID { get; }
    private int balance;
    private Mutex mutex = new Mutex(); 

    public BankAccount(int id, int initialBalance)
    {
        ID = id;
        balance = initialBalance;
    }

    public void Deposit(int amount)
    {
        mutex.WaitOne(); 
        try
        {
            balance += amount;
            Console.WriteLine($"Deposited {amount} into Account {ID}, New Balance: {balance}");
        }
        finally
        {
            mutex.ReleaseMutex(); 
        }
    }

    public void Withdraw(int amount)
    {
        mutex.WaitOne(); 
        try
        {
            if (balance >= amount)
            {
                balance -= amount;
                Console.WriteLine($"Withdrew {amount} from Account {ID}, New Balance: {balance}");
            }
            else
            {
                Console.WriteLine($"Account {ID}: Insufficient funds!");
            }
        }
        finally
        {
            mutex.ReleaseMutex(); 
        }
    }

    public int GetBalance()
    {
        mutex.WaitOne(); 
        try
        {
            return balance;
        }
        finally
        {
            mutex.ReleaseMutex(); 
        }
    }
}

class Bank
{
    private List<BankAccount> accounts = new List<BankAccount>();

    public void AddAccount(int accountId, int initialBalance)
    {
        if (FindAccount(accountId) == null)
        {
            accounts.Add(new BankAccount(accountId, initialBalance));
            Console.WriteLine($"Account {accountId} created with balance {initialBalance}");
        }
        else
        {
            Console.WriteLine("Account ID already exists.");
        }
    }

    public BankAccount FindAccount(int accountId)
    {
        return accounts.Find(account => account.ID == accountId);
    }
}

class Program
{
    static void Main()
    {
        Bank bank = new Bank();

        while (true)
        {
            Console.WriteLine("Choose an option: \n1. Create Account \n2. Deposit \n3. Withdraw \n4. Check Balance \n5. Run Concurrent Transactions \n6. Exit \n");
            string choice = Console.ReadLine();

            if (choice == "6") break;

            switch (choice)
            {
                case "1":
                    Console.Write("Enter new Account ID: ");
                    int newAccountId = int.Parse(Console.ReadLine());
                    Console.Write("Enter initial deposit: ");
                    int initialBalance = int.Parse(Console.ReadLine());
                    bank.AddAccount(newAccountId, initialBalance);
                    break;
                case "2":
                case "3":
                case "4":
                    Console.Write("Enter Account ID: ");
                    int accountId = int.Parse(Console.ReadLine());
                    BankAccount account = bank.FindAccount(accountId);
                    if (account == null)
                    {
                        Console.WriteLine("Account not found.");
                        break;
                    }

                    if (choice == "2")
                    {
                        Console.Write("Enter deposit amount: ");
                        int depositAmount = int.Parse(Console.ReadLine());
                        Thread depositThread = new Thread(() => account.Deposit(depositAmount));
                        depositThread.Start();
                        depositThread.Join();
                    }
                    else if (choice == "3")
                    {
                        Console.Write("Enter withdrawal amount: ");
                        int withdrawAmount = int.Parse(Console.ReadLine());
                        Thread withdrawThread = new Thread(() => account.Withdraw(withdrawAmount));
                        withdrawThread.Start();
                        withdrawThread.Join();
                    }
                    else if (choice == "4")
                    {
                        Console.WriteLine($"Current Balance: {account.GetBalance()}");
                    }
                    break;
                case "5":
                    Console.Write("Enter Account ID: ");
                    int concurrentAccountId = int.Parse(Console.ReadLine());
                    BankAccount concurrentAccount = bank.FindAccount(concurrentAccountId);
                    if (concurrentAccount == null)
                    {
                        Console.WriteLine("Account not found.");
                        break;
                    }

                    Console.Write("Enter number of concurrent transactions: ");
                    int threadCount = int.Parse(Console.ReadLine());

                    List<Thread> threads = new List<Thread>();

                    for (int i = 0; i < threadCount; i++)
                    {
                        Console.Write($"Enter type for thread {i + 1} (deposit/withdraw): ");
                        string transactionType = Console.ReadLine().ToLower();
                        Console.Write("Enter amount: ");
                        int amount = int.Parse(Console.ReadLine());

                        Thread thread;
                        if (transactionType == "deposit")
                        {
                            thread = new Thread(() => concurrentAccount.Deposit(amount));
                        }
                        else if (transactionType == "withdraw")
                        {
                            thread = new Thread(() => concurrentAccount.Withdraw(amount));
                        }
                        else
                        {
                            Console.WriteLine("Invalid transaction type. Skipping...");
                            continue;
                        }

                        threads.Add(thread);
                        thread.Start();
                    }

                    foreach (Thread t in threads)
                    {
                        t.Join();
                    }

                    Console.WriteLine($"Final Balance after concurrent transactions: {concurrentAccount.GetBalance()}");
                    break;
                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    break;
            }
            Console.WriteLine();
        }
    }
}
