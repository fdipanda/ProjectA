# Project A: Multi-Threading Implementation

## Overview

This repository contains a C# application demonstrating a thread-safe banking system simulation. The project showcases concurrent programming concepts using mutexes to prevent race conditions in a multi-threaded environment. The simulation handles multiple simultaneous transactions including deposits, withdrawals, and transfers between accounts.

## Features

- Thread-safe bank account operations using mutex synchronization
- Multiple concurrent transaction support:
  - Deposits
  - Withdrawals
  - Transfers between accounts
- Random transaction generation
- Proper deadlock prevention in transfer operations
- Real-time transaction logging

## Dependencies

- .NET 6.0 SDK or later
- Visual Studio 2022 or Visual Studio Code (recommended)

## Installation

1. Ensure you have the .NET SDK installed. You can download it from [Microsoft's .NET download page](https://dotnet.microsoft.com/download).

2. Clone this repository:
   ```
   git clone https://github.com/fdipanda/ProjectA.git
   cd ProjectA
   ```

## Building the Project

### Using Visual Studio:
1. Open the solution file (`ProjectA.sln`) in Visual Studio
2. Build the solution using `Build > Build Solution` or press `Ctrl+Shift+B`

### Using .NET CLI:
```
dotnet build
```

## Running the Simulation

### Using Visual Studio:
1. Set the project as the startup project
2. Press `F5` to run with debugging, or `Ctrl+F5` to run without debugging

### Using .NET CLI:
```
dotnet run
```

## Expected Output

When you run the program, you'll see console output showing the creation of accounts followed by concurrent transactions from multiple threads. Each transaction will display the thread ID, operation details, and the resulting account balance. At the end, you'll see a confirmation that all transactions have completed.

Sample output:
```
Account 1 created with balance 1000
Account 2 created with balance 1000
Account 3 created with balance 1500
Account 4 created with balance 2000
[Thread 5] Deposited 57 into Account 3. New Balance: 1557
[Thread 11] Deposited 87 into Account 1. New Balance: 1087
[Thread 8] Withdrew 34 from Account 4. New Balance: 1966
[Thread 15] Transferred 45 from Account 2 to Account 3
[Thread 15] Withdrew 45 from Account 2. New Balance: 955
[Thread 15] Deposited 45 into Account 3. New Balance: 1602
...
All transactions completed.
```

## How It Works

1. **BankAccount Class**: Represents a bank account with thread-safe operations:
   - Each account has a unique ID and a balance
   - A mutex ensures only one thread can access an account at a time
   - Methods for deposit, withdrawal, and balance checking

2. **Bank Class**: Manages accounts and provides operations:
   - Maintains a list of bank accounts
   - Allows adding new accounts
   - Handles transfers between accounts using a two-phase locking approach

3. **Main Program**: Sets up the simulation and creates threads:
   - Creates several bank accounts with initial balances
   - Spawns multiple threads for different types of transactions
   - Each thread randomly selects accounts and amounts
   - All threads run concurrently, demonstrating thread safety

## Key Concepts Demonstrated

- Thread synchronization using mutexes
- Preventing race conditions in concurrent operations
- Two-phase locking for complex operations (transfers)
- Resource acquisition and release patterns
- Proper exception handling in multithreaded code

## Extending the Simulation

This simulation can be extended in several ways:

- Add transaction history tracking
- Implement interest calculations
- Add user authentication and authorization
- Create a graphical user interface
- Implement database storage for persistence
- Add more complex banking operations (loans, scheduled payments)
