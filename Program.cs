using System;
using System.Collections.Generic;
using System.Threading;

class BankAccount
{
    public int ID { get; } // Unique account identifier
    private int balance; // Stores account balance
    private Mutex mutex = new Mutex(); // Ensures thread-safe operations

    public BankAccount(int id, int initialBalance)
    {
        ID = id;
        balance = initialBalance;
    }

    // Deposits a specified amount into the account
    public void Deposit(int amount)
    {
        mutex.WaitOne(); // Acquire lock to ensure thread safety
        try
        {
            balance += amount;
            Console.WriteLine($"[Thread {Thread.CurrentThread.ManagedThreadId}] Deposited {amount} into Account {ID}. New Balance: {balance}");
        }
        finally
        {
            mutex.ReleaseMutex(); // Release lock
        }
    }

    // Withdraws a specified amount from the account if sufficient funds are available
    public void Withdraw(int amount)
    {
        mutex.WaitOne(); // Acquire lock before modifying balance
        try
        {
            if (balance >= amount)
            {
                balance -= amount;
                Console.WriteLine($"[Thread {Thread.CurrentThread.ManagedThreadId}] Withdrew {amount} from Account {ID}. New Balance: {balance}");
            }
            else
            {
                Console.WriteLine($"[Thread {Thread.CurrentThread.ManagedThreadId}] Insufficient funds in Account {ID}!");
            }
        }
        finally
        {
            mutex.ReleaseMutex(); // Release lock
        }
    }

    // Retrieves the current account balance
    public int GetBalance()
    {
        mutex.WaitOne(); // Acquire lock to read balance safely
        try
        {
            return balance;
        }
        finally
        {
            mutex.ReleaseMutex(); // Release lock
        }
    }

    // Locks the account for safe multi-threaded operations
    public void Lock() => mutex.WaitOne();
    // Unlocks the account after operations are complete
    public void Unlock() => mutex.ReleaseMutex();
}

class Bank
{
    private List<BankAccount> accounts = new List<BankAccount>(); // List of all bank accounts

    // Adds a new account to the bank if it doesn't already exist
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

    // Finds an account by its ID
    public BankAccount FindAccount(int accountId)
    {
        return accounts.Find(account => account.ID == accountId);
    }

    // Transfers a specified amount from one account to another
    public void Transfer(BankAccount from, BankAccount to, int amount)
    {
        from.Lock(); // Lock source account
        to.Lock(); // Lock destination account

        try
        {
            if (from.GetBalance() >= amount)
            {
                from.Withdraw(amount);
                to.Deposit(amount);
                Console.WriteLine($"[Thread {Thread.CurrentThread.ManagedThreadId}] Transferred {amount} from Account {from.ID} to Account {to.ID}");
            }
            else
            {
                Console.WriteLine($"[Thread {Thread.CurrentThread.ManagedThreadId}] Transfer failed: Insufficient funds in Account {from.ID}");
            }
        }
        finally
        {
            from.Unlock(); // Unlock source account
            to.Unlock(); // Unlock destination account
        }
    }
}

class Program
{
    static void Main()
    {
        Bank bank = new Bank();
        
        // Create bank accounts
        bank.AddAccount(1, 1000);
        bank.AddAccount(2, 1000);
        bank.AddAccount(3, 1500);
        bank.AddAccount(4, 2000);

        Random rand = new Random();
        List<Thread> threads = new List<Thread>();

        // Create 10 threads for deposits
        for (int i = 0; i < 10; i++)
        {
            threads.Add(new Thread(() =>
            {
                BankAccount acc = bank.FindAccount(rand.Next(1, 5));
                acc.Deposit(rand.Next(10, 100));
            }));
        }

        // Create 5 threads for withdrawals
        for (int i = 0; i < 5; i++)
        {
            threads.Add(new Thread(() =>
            {
                BankAccount acc = bank.FindAccount(rand.Next(1, 5));
                acc.Withdraw(rand.Next(10, 100));
            }));
        }

        // Create 5 threads for transfers
        for (int i = 0; i < 5; i++)
        {
            threads.Add(new Thread(() =>
            {
                BankAccount from = bank.FindAccount(rand.Next(1, 5));
                BankAccount to;
                do
                {
                    to = bank.FindAccount(rand.Next(1, 5));
                } while (from.ID == to.ID); // Ensure the source and destination accounts are different

                bank.Transfer(from, to, rand.Next(10, 100));
            }));
        }

        // Start all threads
        foreach (var thread in threads)
        {
            thread.Start();
        }

        // Wait for all threads to finish execution
        foreach (var thread in threads)
        {
            thread.Join();
        }

        Console.WriteLine("All transactions completed.");
    }
}
