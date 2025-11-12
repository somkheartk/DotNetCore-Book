# บทที่ 3: พื้นฐาน C#

## 3.1 ตัวแปรและชนิดข้อมูล (Variables and Data Types)

### 3.1.1 Value Types

```csharp
// Integer Types
byte myByte = 255;              // 0 to 255
sbyte mySByte = -128;           // -128 to 127
short myShort = -32768;         // -32,768 to 32,767
ushort myUShort = 65535;        // 0 to 65,535
int myInt = -2147483648;        // -2,147,483,648 to 2,147,483,647
uint myUInt = 4294967295;       // 0 to 4,294,967,295
long myLong = -9223372036854775808L;    // ขนาดใหญ่มาก
ulong myULong = 18446744073709551615UL; // ขนาดใหญ่มาก

// Floating Point Types
float myFloat = 3.14f;          // 7 digits precision
double myDouble = 3.14159265359; // 15-16 digits precision
decimal myDecimal = 3.14159265359m; // 28-29 digits precision (สำหรับการเงิน)

// Boolean
bool isTrue = true;
bool isFalse = false;

// Character
char myChar = 'A';
char thaiChar = 'ก';

// DateTime
DateTime now = DateTime.Now;
DateTime today = DateTime.Today;
```

### 3.1.2 Reference Types

```csharp
// String
string greeting = "สวัสดี .NET Core";
string multiLine = @"บรรทัดที่ 1
บรรทัดที่ 2
บรรทัดที่ 3";

string interpolated = $"ชื่อ: {name}, อายุ: {age}";

// Array
int[] numbers = { 1, 2, 3, 4, 5 };
string[] names = new string[3];
names[0] = "สมชาย";
names[1] = "สมหญิง";
names[2] = "สมศักดิ์";

// Object
object obj = 100;
object obj2 = "Hello";
```

### 3.1.3 Nullable Types

```csharp
// Nullable value type
int? nullableInt = null;
nullableInt = 10;

// Null-coalescing operator
int value = nullableInt ?? 0; // ถ้า null ใช้ 0

// Null-conditional operator
string? name = null;
int? length = name?.Length; // ถ้า name เป็น null จะได้ null
```

### 3.1.4 var และ Type Inference

```csharp
var number = 10;           // int
var text = "Hello";        // string
var price = 99.99m;        // decimal
var list = new List<int>(); // List<int>

// ไม่สามารถใช้ var โดยไม่มี initialization
// var x; // Error!
```

### 3.1.5 Constants

```csharp
const double PI = 3.14159;
const string APP_NAME = "My Application";
const int MAX_USERS = 1000;

// const ต้องกำหนดค่าตอน compile time
// readonly สามารถกำหนดค่าตอน runtime
```

## 3.2 Operators

### 3.2.1 Arithmetic Operators

```csharp
int a = 10, b = 3;

int sum = a + b;        // 13
int diff = a - b;       // 7
int product = a * b;    // 30
int quotient = a / b;   // 3
int remainder = a % b;  // 1

// Increment/Decrement
int x = 5;
x++;  // x = 6 (post-increment)
++x;  // x = 7 (pre-increment)
x--;  // x = 6 (post-decrement)
--x;  // x = 5 (pre-decrement)
```

### 3.2.2 Comparison Operators

```csharp
int x = 10, y = 5;

bool isEqual = (x == y);        // false
bool isNotEqual = (x != y);     // true
bool isGreater = (x > y);       // true
bool isLess = (x < y);          // false
bool isGreaterOrEqual = (x >= y); // true
bool isLessOrEqual = (x <= y);    // false
```

### 3.2.3 Logical Operators

```csharp
bool a = true, b = false;

bool and = a && b;  // false (AND)
bool or = a || b;   // true (OR)
bool not = !a;      // false (NOT)

// Short-circuit evaluation
if (x > 0 && y / x > 2) // ถ้า x > 0 เป็น false จะไม่ check y / x
{
    // ...
}
```

### 3.2.4 Assignment Operators

```csharp
int x = 10;

x += 5;  // x = x + 5 (15)
x -= 3;  // x = x - 3 (12)
x *= 2;  // x = x * 2 (24)
x /= 4;  // x = x / 4 (6)
x %= 4;  // x = x % 4 (2)
```

## 3.3 Control Flow

### 3.3.1 if-else

```csharp
int score = 85;

if (score >= 80)
{
    Console.WriteLine("เกรด A");
}
else if (score >= 70)
{
    Console.WriteLine("เกรด B");
}
else if (score >= 60)
{
    Console.WriteLine("เกรด C");
}
else if (score >= 50)
{
    Console.WriteLine("เกรด D");
}
else
{
    Console.WriteLine("เกรด F");
}

// Ternary operator
string result = (score >= 50) ? "ผ่าน" : "ไม่ผ่าน";
```

### 3.3.2 switch

```csharp
int day = 3;
string dayName;

switch (day)
{
    case 1:
        dayName = "จันทร์";
        break;
    case 2:
        dayName = "อังคาร";
        break;
    case 3:
        dayName = "พุธ";
        break;
    case 4:
        dayName = "พฤหัสบดี";
        break;
    case 5:
        dayName = "ศุกร์";
        break;
    case 6:
        dayName = "เสาร์";
        break;
    case 7:
        dayName = "อาทิตย์";
        break;
    default:
        dayName = "ไม่ถูกต้อง";
        break;
}

// Switch expression (C# 8+)
string dayName2 = day switch
{
    1 => "จันทร์",
    2 => "อังคาร",
    3 => "พุธ",
    4 => "พฤหัสบดี",
    5 => "ศุกร์",
    6 => "เสาร์",
    7 => "อาทิตย์",
    _ => "ไม่ถูกต้อง"
};
```

### 3.3.3 Loops

**for Loop:**

```csharp
for (int i = 0; i < 5; i++)
{
    Console.WriteLine($"รอบที่ {i + 1}");
}

// Loop ย้อนกลับ
for (int i = 5; i > 0; i--)
{
    Console.WriteLine(i);
}
```

**while Loop:**

```csharp
int count = 0;
while (count < 5)
{
    Console.WriteLine($"Count: {count}");
    count++;
}
```

**do-while Loop:**

```csharp
int number;
do
{
    Console.Write("ใส่ตัวเลข 1-10: ");
    number = int.Parse(Console.ReadLine());
} while (number < 1 || number > 10);
```

**foreach Loop:**

```csharp
string[] fruits = { "แอปเปิล", "กล้วย", "ส้ม", "มะม่วง" };

foreach (string fruit in fruits)
{
    Console.WriteLine(fruit);
}

// กับ List
List<int> numbers = new List<int> { 1, 2, 3, 4, 5 };
foreach (int num in numbers)
{
    Console.WriteLine(num);
}
```

### 3.3.4 break และ continue

```csharp
// break - หยุด loop
for (int i = 0; i < 10; i++)
{
    if (i == 5)
        break; // หยุดที่ i = 5
    Console.WriteLine(i);
}

// continue - ข้าม iteration นี้
for (int i = 0; i < 10; i++)
{
    if (i % 2 == 0)
        continue; // ข้ามเลขคู่
    Console.WriteLine(i); // แสดงเฉพาะเลขคี่
}
```

## 3.4 Methods และ Functions

### 3.4.1 Method พื้นฐาน

```csharp
// Method ไม่มี return value
void PrintGreeting(string name)
{
    Console.WriteLine($"สวัสดี {name}!");
}

// Method มี return value
int Add(int a, int b)
{
    return a + b;
}

// Method มีหลาย parameters
double Calculate(double num1, double num2, string operation)
{
    return operation switch
    {
        "+" => num1 + num2,
        "-" => num1 - num2,
        "*" => num1 * num2,
        "/" => num2 != 0 ? num1 / num2 : 0,
        _ => 0
    };
}

// เรียกใช้
PrintGreeting("สมชาย");
int result = Add(5, 3);
double calcResult = Calculate(10, 5, "+");
```

### 3.4.2 Optional Parameters

```csharp
void PrintMessage(string message, int times = 1)
{
    for (int i = 0; i < times; i++)
    {
        Console.WriteLine(message);
    }
}

// เรียกใช้
PrintMessage("Hello");           // times = 1 (default)
PrintMessage("Hello", 3);        // times = 3
```

### 3.4.3 Named Arguments

```csharp
void CreateUser(string name, int age, string city)
{
    Console.WriteLine($"Name: {name}, Age: {age}, City: {city}");
}

// เรียกใช้ด้วย named arguments
CreateUser(age: 25, name: "สมชาย", city: "กรุงเทพฯ");
```

### 3.4.4 out Parameters

```csharp
bool TryDivide(int dividend, int divisor, out double result)
{
    if (divisor == 0)
    {
        result = 0;
        return false;
    }
    
    result = (double)dividend / divisor;
    return true;
}

// เรียกใช้
if (TryDivide(10, 3, out double quotient))
{
    Console.WriteLine($"ผลลัพธ์: {quotient}");
}
```

### 3.4.5 ref Parameters

```csharp
void Swap(ref int a, ref int b)
{
    int temp = a;
    a = b;
    b = temp;
}

// เรียกใช้
int x = 5, y = 10;
Console.WriteLine($"Before: x={x}, y={y}");
Swap(ref x, ref y);
Console.WriteLine($"After: x={x}, y={y}");
```

### 3.4.6 Expression-bodied Members

```csharp
// Method
int Square(int x) => x * x;

// Property
string FullName => $"{FirstName} {LastName}";

// Constructor
public Person(string name) => Name = name;
```

## 3.5 Object-Oriented Programming (OOP)

### 3.5.1 Classes และ Objects

```csharp
// คำนิยาม Class
public class Person
{
    // Fields (private)
    private string name;
    private int age;
    
    // Properties (public)
    public string Name
    {
        get { return name; }
        set { name = value; }
    }
    
    public int Age
    {
        get { return age; }
        set 
        { 
            if (value >= 0)
                age = value; 
        }
    }
    
    // Auto-implemented properties
    public string City { get; set; }
    
    // Constructor
    public Person(string name, int age)
    {
        this.name = name;
        this.age = age;
    }
    
    // Methods
    public void Introduce()
    {
        Console.WriteLine($"สวัสดี ฉันชื่อ {Name} อายุ {Age} ปี");
    }
    
    public int GetBirthYear()
    {
        return DateTime.Now.Year - Age;
    }
}

// สร้าง Object
Person person = new Person("สมชาย", 25);
person.City = "กรุงเทพฯ";
person.Introduce();
Console.WriteLine($"ปีเกิด: {person.GetBirthYear()}");
```

### 3.5.2 Inheritance (การสืบทอด)

```csharp
// Base class
public class Animal
{
    public string Name { get; set; }
    public int Age { get; set; }
    
    public virtual void MakeSound()
    {
        Console.WriteLine("สัตว์ส่งเสียง");
    }
    
    public void Eat()
    {
        Console.WriteLine($"{Name} กำลังกิน");
    }
}

// Derived class
public class Dog : Animal
{
    public string Breed { get; set; }
    
    public override void MakeSound()
    {
        Console.WriteLine("โฮ่ง โฮ่ง!");
    }
    
    public void Fetch()
    {
        Console.WriteLine($"{Name} กำลังเก็บของ");
    }
}

public class Cat : Animal
{
    public override void MakeSound()
    {
        Console.WriteLine("เหมียว เหมียว!");
    }
}

// ใช้งาน
Dog dog = new Dog { Name = "บัดดี้", Age = 3, Breed = "Golden Retriever" };
dog.MakeSound();  // โฮ่ง โฮ่ง!
dog.Eat();        // บัดดี้ กำลังกิน
dog.Fetch();      // บัดดี้ กำลังเก็บของ

Cat cat = new Cat { Name = "มิ้ว", Age = 2 };
cat.MakeSound();  // เหมียว เหมียว!
```

### 3.5.3 Polymorphism

```csharp
// ใช้ Base class reference เก็บ Derived class object
Animal animal1 = new Dog { Name = "บัดดี้" };
Animal animal2 = new Cat { Name = "มิ้ว" };

animal1.MakeSound(); // โฮ่ง โฮ่ง!
animal2.MakeSound(); // เหมียว เหมียว!

// Array ของ Animals
Animal[] animals = 
{
    new Dog { Name = "บัดดี้" },
    new Cat { Name = "มิ้ว" },
    new Dog { Name = "แม็กซ์" }
};

foreach (Animal animal in animals)
{
    animal.MakeSound();
}
```

### 3.5.4 Encapsulation

```csharp
public class BankAccount
{
    // Private field
    private decimal balance;
    
    // Public property with validation
    public decimal Balance
    {
        get { return balance; }
        private set { balance = value; }
    }
    
    public string AccountNumber { get; private set; }
    
    public BankAccount(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        balance = initialBalance;
    }
    
    // Public methods
    public bool Deposit(decimal amount)
    {
        if (amount > 0)
        {
            balance += amount;
            Console.WriteLine($"ฝากเงิน {amount:C} สำเร็จ");
            return true;
        }
        return false;
    }
    
    public bool Withdraw(decimal amount)
    {
        if (amount > 0 && amount <= balance)
        {
            balance -= amount;
            Console.WriteLine($"ถอนเงิน {amount:C} สำเร็จ");
            return true;
        }
        Console.WriteLine("ยอดเงินไม่เพียงพอ");
        return false;
    }
    
    public void PrintBalance()
    {
        Console.WriteLine($"ยอดคงเหลือ: {balance:C}");
    }
}

// ใช้งาน
BankAccount account = new BankAccount("001-1234567", 1000);
account.Deposit(500);
account.Withdraw(200);
account.PrintBalance();
```

### 3.5.5 Abstract Classes

```csharp
public abstract class Shape
{
    public string Name { get; set; }
    
    // Abstract method (ต้อง override)
    public abstract double CalculateArea();
    
    // Concrete method
    public void Display()
    {
        Console.WriteLine($"รูป: {Name}, พื้นที่: {CalculateArea():F2}");
    }
}

public class Circle : Shape
{
    public double Radius { get; set; }
    
    public override double CalculateArea()
    {
        return Math.PI * Radius * Radius;
    }
}

public class Rectangle : Shape
{
    public double Width { get; set; }
    public double Height { get; set; }
    
    public override double CalculateArea()
    {
        return Width * Height;
    }
}

// ใช้งาน
Circle circle = new Circle { Name = "วงกลม", Radius = 5 };
Rectangle rect = new Rectangle { Name = "สี่เหลี่ยม", Width = 4, Height = 6 };

circle.Display();
rect.Display();
```

### 3.5.6 Interfaces

```csharp
public interface IPayable
{
    decimal CalculatePayment();
    void ProcessPayment();
}

public class Employee : IPayable
{
    public string Name { get; set; }
    public decimal Salary { get; set; }
    
    public decimal CalculatePayment()
    {
        return Salary;
    }
    
    public void ProcessPayment()
    {
        Console.WriteLine($"จ่ายเงินเดือนให้ {Name}: {Salary:C}");
    }
}

public class Freelancer : IPayable
{
    public string Name { get; set; }
    public decimal HourlyRate { get; set; }
    public int HoursWorked { get; set; }
    
    public decimal CalculatePayment()
    {
        return HourlyRate * HoursWorked;
    }
    
    public void ProcessPayment()
    {
        Console.WriteLine($"จ่ายเงินให้ {Name}: {CalculatePayment():C}");
    }
}

// ใช้งาน
List<IPayable> payables = new List<IPayable>
{
    new Employee { Name = "สมชาย", Salary = 30000 },
    new Freelancer { Name = "สมศรี", HourlyRate = 500, HoursWorked = 40 }
};

foreach (var payable in payables)
{
    payable.ProcessPayment();
}
```

## 3.6 Collections

### 3.6.1 List<T>

```csharp
// สร้าง List
List<string> names = new List<string>();

// เพิ่มข้อมูล
names.Add("สมชาย");
names.Add("สมหญิง");
names.Add("สมศักดิ์");

// เพิ่มหลายรายการ
names.AddRange(new[] { "สมพร", "สมใจ" });

// เข้าถึงข้อมูล
Console.WriteLine(names[0]); // สมชาย

// นับจำนวน
Console.WriteLine($"จำนวน: {names.Count}");

// ลบข้อมูล
names.Remove("สมชาย");
names.RemoveAt(0);

// ตรวจสอบ
bool contains = names.Contains("สมหญิง");

// Loop
foreach (string name in names)
{
    Console.WriteLine(name);
}
```

### 3.6.2 Dictionary<TKey, TValue>

```csharp
// สร้าง Dictionary
Dictionary<string, int> scores = new Dictionary<string, int>();

// เพิ่มข้อมูล
scores.Add("สมชาย", 85);
scores["สมหญิง"] = 92;
scores["สมศักดิ์"] = 78;

// เข้าถึงข้อมูล
int score = scores["สมชาย"];

// ตรวจสอบ key
if (scores.ContainsKey("สมชาย"))
{
    Console.WriteLine($"คะแนนของสมชาย: {scores["สมชาย"]}");
}

// TryGetValue (ปลอดภัยกว่า)
if (scores.TryGetValue("สมพร", out int somponScore))
{
    Console.WriteLine($"คะแนน: {somponScore}");
}
else
{
    Console.WriteLine("ไม่พบข้อมูล");
}

// Loop
foreach (KeyValuePair<string, int> item in scores)
{
    Console.WriteLine($"{item.Key}: {item.Value}");
}

// หรือ
foreach (var item in scores)
{
    Console.WriteLine($"{item.Key}: {item.Value}");
}
```

### 3.6.3 HashSet<T>

```csharp
// สร้าง HashSet (ไม่มีข้อมูลซ้ำ)
HashSet<string> uniqueNames = new HashSet<string>();

uniqueNames.Add("สมชาย");
uniqueNames.Add("สมหญิง");
uniqueNames.Add("สมชาย"); // ไม่เพิ่ม เพราะซ้ำ

Console.WriteLine($"จำนวน: {uniqueNames.Count}"); // 2

// Set operations
HashSet<int> set1 = new HashSet<int> { 1, 2, 3, 4 };
HashSet<int> set2 = new HashSet<int> { 3, 4, 5, 6 };

set1.UnionWith(set2);        // Union: {1, 2, 3, 4, 5, 6}
set1.IntersectWith(set2);    // Intersection: {3, 4}
set1.ExceptWith(set2);       // Difference: {1, 2}
```

### 3.6.4 Queue<T>

```csharp
// FIFO (First In First Out)
Queue<string> queue = new Queue<string>();

queue.Enqueue("คนที่ 1");
queue.Enqueue("คนที่ 2");
queue.Enqueue("คนที่ 3");

Console.WriteLine($"คนแรกในคิว: {queue.Peek()}");

while (queue.Count > 0)
{
    string person = queue.Dequeue();
    Console.WriteLine($"ให้บริการ: {person}");
}
```

### 3.6.5 Stack<T>

```csharp
// LIFO (Last In First Out)
Stack<string> stack = new Stack<string>();

stack.Push("จาน 1");
stack.Push("จาน 2");
stack.Push("จาน 3");

Console.WriteLine($"จานบนสุด: {stack.Peek()}");

while (stack.Count > 0)
{
    string plate = stack.Pop();
    Console.WriteLine($"เอาจานออก: {plate}");
}
```

## 3.7 LINQ (Language Integrated Query)

### 3.7.1 LINQ พื้นฐาน

```csharp
List<int> numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

// Where - กรองข้อมูล
var evenNumbers = numbers.Where(n => n % 2 == 0);
Console.WriteLine(string.Join(", ", evenNumbers)); // 2, 4, 6, 8, 10

// Select - แปลงข้อมูล
var squared = numbers.Select(n => n * n);
Console.WriteLine(string.Join(", ", squared)); // 1, 4, 9, 16, ...

// OrderBy - เรียงลำดับ
var ordered = numbers.OrderByDescending(n => n);
Console.WriteLine(string.Join(", ", ordered)); // 10, 9, 8, ...

// First, Last
int first = numbers.First(); // 1
int last = numbers.Last();   // 10
int firstEven = numbers.First(n => n % 2 == 0); // 2

// Any, All
bool hasEven = numbers.Any(n => n % 2 == 0);  // true
bool allPositive = numbers.All(n => n > 0);   // true

// Count, Sum, Average
int count = numbers.Count();      // 10
int sum = numbers.Sum();          // 55
double avg = numbers.Average();   // 5.5

// Max, Min
int max = numbers.Max(); // 10
int min = numbers.Min(); // 1
```

### 3.7.2 LINQ กับ Objects

```csharp
class Student
{
    public string Name { get; set; }
    public int Age { get; set; }
    public double GPA { get; set; }
}

List<Student> students = new List<Student>
{
    new Student { Name = "สมชาย", Age = 20, GPA = 3.5 },
    new Student { Name = "สมหญิง", Age = 21, GPA = 3.8 },
    new Student { Name = "สมศักดิ์", Age = 19, GPA = 3.2 },
    new Student { Name = "สมพร", Age = 22, GPA = 3.9 }
};

// กรองนักเรียนที่ GPA >= 3.5
var topStudents = students.Where(s => s.GPA >= 3.5);

// เรียงตาม GPA จากมากไปน้อย
var sortedStudents = students.OrderByDescending(s => s.GPA);

// เลือกเฉพาะชื่อ
var names = students.Select(s => s.Name);

// Group by
var groupedByAge = students.GroupBy(s => s.Age);
foreach (var group in groupedByAge)
{
    Console.WriteLine($"อายุ {group.Key}:");
    foreach (var student in group)
    {
        Console.WriteLine($"  - {student.Name}");
    }
}

// คำนวณค่าเฉลี่ย GPA
double avgGPA = students.Average(s => s.GPA);
Console.WriteLine($"GPA เฉลี่ย: {avgGPA:F2}");
```

## 3.8 Exception Handling

### 3.8.1 try-catch-finally

```csharp
try
{
    Console.Write("ใส่ตัวเลข: ");
    int number = int.Parse(Console.ReadLine());
    int result = 100 / number;
    Console.WriteLine($"ผลลัพธ์: {result}");
}
catch (FormatException)
{
    Console.WriteLine("กรุณาใส่ตัวเลขที่ถูกต้อง");
}
catch (DivideByZeroException)
{
    Console.WriteLine("ไม่สามารถหารด้วย 0 ได้");
}
catch (Exception ex)
{
    Console.WriteLine($"เกิดข้อผิดพลาด: {ex.Message}");
}
finally
{
    Console.WriteLine("การทำงานเสร็จสิ้น");
}
```

### 3.8.2 throw

```csharp
public class Account
{
    public decimal Balance { get; private set; }
    
    public void Withdraw(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("จำนวนเงินต้องมากกว่า 0");
            
        if (amount > Balance)
            throw new InvalidOperationException("ยอดเงินไม่เพียงพอ");
            
        Balance -= amount;
    }
}
```

### 3.8.3 Custom Exceptions

```csharp
public class InsufficientFundsException : Exception
{
    public decimal Balance { get; }
    public decimal RequiredAmount { get; }
    
    public InsufficientFundsException(decimal balance, decimal required)
        : base($"ยอดเงินไม่เพียงพอ มี {balance:C} ต้องการ {required:C}")
    {
        Balance = balance;
        RequiredAmount = required;
    }
}

// ใช้งาน
try
{
    throw new InsufficientFundsException(1000, 1500);
}
catch (InsufficientFundsException ex)
{
    Console.WriteLine(ex.Message);
    Console.WriteLine($"ขาดอีก {ex.RequiredAmount - ex.Balance:C}");
}
```

## 3.9 สรุป

ในบทนี้เราได้เรียนรู้:

✅ ตัวแปรและชนิดข้อมูลใน C#
✅ Operators ต่างๆ
✅ Control Flow (if, switch, loops)
✅ Methods และ Functions
✅ Object-Oriented Programming
✅ Collections (List, Dictionary, etc.)
✅ LINQ สำหรับ query ข้อมูล
✅ Exception Handling

## 📚 แบบฝึกหัด

1. สร้าง Class `Book` ที่มี properties: Title, Author, Price, ISBN
2. สร้าง List ของหนังสือและใช้ LINQ หาหนังสือที่ราคามากกว่า 300 บาท
3. สร้าง Class `Calculator` ที่มี methods สำหรับการคำนวณพื้นฐาน
4. เขียนโปรแกรมจัดการรายชื่อนักเรียนโดยใช้ Dictionary
5. สร้าง Abstract Class `Vehicle` และ Derived Classes: `Car`, `Motorcycle`

---

**ก่อนหน้า:** [บทที่ 2 - การติดตั้งและเริ่มต้นใช้งาน](../Chapter02-GettingStarted/README.md)
**ต่อไป:** [บทที่ 4 - สร้าง Console Application แรก](../Chapter04-ConsoleApplication/README.md)
