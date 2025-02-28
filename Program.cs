using System;
using System.Collections.Generic;
using System.Threading;

class BankAccount
{
    private int balance;
    private readonly object lockObject = new object(); // Mutex alternative

    public BankAccount(int initialBalance)
    {
        balance = initialBalance;
    }

    public void Deposit(int amount)
    {
        lock (lockObject)
        {
            balance += amount;
            Console.WriteLine($"Deposited {amount}, New Balance: {balance}");
        }
    }

    public void Withdraw(int amount)
    {
        lock (lockObject)
        {
            if (balance >= amount)
            {
                balance -= amount;
                Console.WriteLine($"Withdrew {amount}, New Balance: {balance}");
            }
            else
            {
                Console.WriteLine("Insufficient funds!");
            }
        }
    }

    public int GetBalance()
    {
        lock (lockObject)
        {
            return balance;
        }
    }
}

class Bank
{
    private Dictionary<int, BankAccount> accounts = new Dictionary<int, BankAccount>();

    public void AddAccount(int accountId, int initialBalance)
    {
        if (!accounts.ContainsKey(accountId))
        {
            accounts[accountId] = new BankAccount(initialBalance);
            Console.WriteLine($"Account {accountId} created with balance {initialBalance}");
        }
        else
        {
            Console.WriteLine("Account ID already exists.");
        }
    }

    public BankAccount GetAccount(int accountId)
    {
        return accounts.ContainsKey(accountId) ? accounts[accountId] : null;
    }
}

class Program
{
    static void Main()
    {
        Bank bank = new Bank();

        while (true)
        {
            Console.WriteLine("Choose an option: \n1. Create Account \n2. Deposit \n3. Withdraw \n4. Check Balance \n5. Exit \n");
            string choice = Console.ReadLine();

            if (choice == "5") break;

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
                    BankAccount account = bank.GetAccount(accountId);
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
                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    break;
            }
            Console.WriteLine();

        }
    }
}
