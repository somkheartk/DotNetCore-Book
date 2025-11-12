# บทที่ 4: สร้าง Console Application แรก

## 4.1 สร้างโปรเจคแรก

### 4.1.1 ใช้ dotnet CLI

```bash
# สร้าง directory สำหรับโปรเจค
mkdir StudentManagementApp
cd StudentManagementApp

# สร้าง console application
dotnet new console -n StudentManagement

# เข้าไปในโฟลเดอร์
cd StudentManagement

# รันโปรแกรม
dotnet run
```

### 4.1.2 โครงสร้างโปรเจค

```
StudentManagement/
├── Program.cs
├── StudentManagement.csproj
├── bin/
└── obj/
```

## 4.2 โปรแกรมจัดการข้อมูลนักเรียน

### 4.2.1 สร้าง Student Class

สร้างไฟล์ `Student.cs`:

```csharp
namespace StudentManagement
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public double GPA { get; set; }
        public string Major { get; set; }

        public Student(int id, string name, int age, double gpa, string major)
        {
            Id = id;
            Name = name;
            Age = age;
            GPA = gpa;
            Major = major;
        }

        public void DisplayInfo()
        {
            Console.WriteLine($"ID: {Id}");
            Console.WriteLine($"ชื่อ: {Name}");
            Console.WriteLine($"อายุ: {Age} ปี");
            Console.WriteLine($"GPA: {GPA:F2}");
            Console.WriteLine($"สาขา: {Major}");
        }

        public string GetGrade()
        {
            return GPA switch
            {
                >= 3.5 => "ดีมาก",
                >= 3.0 => "ดี",
                >= 2.5 => "ปานกลาง",
                >= 2.0 => "พอใช้",
                _ => "ปรับปรุง"
            };
        }

        public override string ToString()
        {
            return $"[{Id}] {Name} - {Major} (GPA: {GPA:F2})";
        }
    }
}
```

### 4.2.2 สร้าง StudentManager Class

สร้างไฟล์ `StudentManager.cs`:

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

namespace StudentManagement
{
    public class StudentManager
    {
        private List<Student> students;
        private int nextId;

        public StudentManager()
        {
            students = new List<Student>();
            nextId = 1;
            LoadSampleData();
        }

        private void LoadSampleData()
        {
            AddStudent("สมชาย ใจดี", 20, 3.5, "วิศวกรรมคอมพิวเตอร์");
            AddStudent("สมหญิง รักเรียน", 21, 3.8, "วิทยาศาสตร์คอมพิวเตอร์");
            AddStudent("สมศักดิ์ ขยัน", 19, 3.2, "เทคโนโลยีสารสนเทศ");
        }

        public void AddStudent(string name, int age, double gpa, string major)
        {
            var student = new Student(nextId++, name, age, gpa, major);
            students.Add(student);
            Console.WriteLine($"\n✓ เพิ่มนักเรียน {name} เรียบร้อยแล้ว");
        }

        public void DisplayAllStudents()
        {
            if (students.Count == 0)
            {
                Console.WriteLine("\nไม่มีข้อมูลนักเรียน");
                return;
            }

            Console.WriteLine("\n╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║           รายชื่อนักเรียนทั้งหมด                  ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");

            foreach (var student in students)
            {
                Console.WriteLine($"\n{student}");
                Console.WriteLine($"  อายุ: {student.Age} ปี | ผลการเรียน: {student.GetGrade()}");
            }
            Console.WriteLine($"\nจำนวนนักเรียนทั้งหมด: {students.Count} คน");
        }

        public void SearchStudent(int id)
        {
            var student = students.FirstOrDefault(s => s.Id == id);
            
            if (student != null)
            {
                Console.WriteLine("\n╔════════════════════════════════════════════════════╗");
                Console.WriteLine("║              ข้อมูลนักเรียน                        ║");
                Console.WriteLine("╚════════════════════════════════════════════════════╝\n");
                student.DisplayInfo();
                Console.WriteLine($"ผลการเรียน: {student.GetGrade()}");
            }
            else
            {
                Console.WriteLine($"\n✗ ไม่พบนักเรียน ID: {id}");
            }
        }

        public void SearchByName(string name)
        {
            var results = students.Where(s => 
                s.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();

            if (results.Count > 0)
            {
                Console.WriteLine($"\nพบนักเรียน {results.Count} คน:");
                foreach (var student in results)
                {
                    Console.WriteLine($"  {student}");
                }
            }
            else
            {
                Console.WriteLine($"\n✗ ไม่พบนักเรียนชื่อ: {name}");
            }
        }

        public void UpdateStudent(int id)
        {
            var student = students.FirstOrDefault(s => s.Id == id);
            
            if (student == null)
            {
                Console.WriteLine($"\n✗ ไม่พบนักเรียน ID: {id}");
                return;
            }

            Console.WriteLine($"\nแก้ไขข้อมูล: {student.Name}");
            Console.WriteLine("(กด Enter เพื่อข้ามการแก้ไข)");

            Console.Write($"ชื่อใหม่ [{student.Name}]: ");
            string? newName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newName))
                student.Name = newName;

            Console.Write($"อายุใหม่ [{student.Age}]: ");
            string? ageInput = Console.ReadLine();
            if (int.TryParse(ageInput, out int newAge))
                student.Age = newAge;

            Console.Write($"GPA ใหม่ [{student.GPA:F2}]: ");
            string? gpaInput = Console.ReadLine();
            if (double.TryParse(gpaInput, out double newGPA))
                student.GPA = newGPA;

            Console.Write($"สาขาใหม่ [{student.Major}]: ");
            string? newMajor = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newMajor))
                student.Major = newMajor;

            Console.WriteLine($"\n✓ อัปเดตข้อมูลเรียบร้อยแล้ว");
        }

        public void DeleteStudent(int id)
        {
            var student = students.FirstOrDefault(s => s.Id == id);
            
            if (student != null)
            {
                Console.Write($"\nยืนยันการลบ {student.Name}? (y/n): ");
                string? confirm = Console.ReadLine()?.ToLower();
                
                if (confirm == "y" || confirm == "yes")
                {
                    students.Remove(student);
                    Console.WriteLine($"✓ ลบนักเรียนเรียบร้อยแล้ว");
                }
                else
                {
                    Console.WriteLine("ยกเลิกการลบ");
                }
            }
            else
            {
                Console.WriteLine($"\n✗ ไม่พบนักเรียน ID: {id}");
            }
        }

        public void DisplayStatistics()
        {
            if (students.Count == 0)
            {
                Console.WriteLine("\nไม่มีข้อมูลสำหรับแสดงสถิติ");
                return;
            }

            Console.WriteLine("\n╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║              สถิตินักเรียน                         ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");

            Console.WriteLine($"\nจำนวนนักเรียนทั้งหมด: {students.Count} คน");
            Console.WriteLine($"อายุเฉลี่ย: {students.Average(s => s.Age):F1} ปี");
            Console.WriteLine($"GPA เฉลี่ย: {students.Average(s => s.GPA):F2}");
            Console.WriteLine($"GPA สูงสุด: {students.Max(s => s.GPA):F2}");
            Console.WriteLine($"GPA ต่ำสุด: {students.Min(s => s.GPA):F2}");

            var topStudent = students.OrderByDescending(s => s.GPA).First();
            Console.WriteLine($"\nนักเรียนที่ได้ GPA สูงสุด: {topStudent.Name} ({topStudent.GPA:F2})");

            // จำนวนนักเรียนแต่ละสาขา
            Console.WriteLine("\nจำนวนนักเรียนแต่ละสาขา:");
            var byMajor = students.GroupBy(s => s.Major);
            foreach (var group in byMajor)
            {
                Console.WriteLine($"  - {group.Key}: {group.Count()} คน");
            }

            // จำนวนตามผลการเรียน
            Console.WriteLine("\nจำนวนตามผลการเรียน:");
            var byGrade = students.GroupBy(s => s.GetGrade());
            foreach (var group in byGrade.OrderByDescending(g => g.Key))
            {
                Console.WriteLine($"  - {group.Key}: {group.Count()} คน");
            }
        }

        public void DisplayTopStudents(int count = 5)
        {
            var topStudents = students.OrderByDescending(s => s.GPA).Take(count).ToList();

            Console.WriteLine($"\n╔════════════════════════════════════════════════════╗");
            Console.WriteLine($"║          นักเรียน Top {count}                            ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");

            int rank = 1;
            foreach (var student in topStudents)
            {
                Console.WriteLine($"\n{rank}. {student.Name}");
                Console.WriteLine($"   สาขา: {student.Major} | GPA: {student.GPA:F2}");
                rank++;
            }
        }
    }
}
```

### 4.2.3 Main Program (Program.cs)

```csharp
using System;

namespace StudentManagement
{
    class Program
    {
        static void Main(string[] args)
        {
            var manager = new StudentManager();
            bool running = true;

            Console.OutputEncoding = System.Text.Encoding.UTF8;

            while (running)
            {
                DisplayMenu();
                string? choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            AddNewStudent(manager);
                            break;
                        case "2":
                            manager.DisplayAllStudents();
                            break;
                        case "3":
                            SearchStudentById(manager);
                            break;
                        case "4":
                            SearchStudentByName(manager);
                            break;
                        case "5":
                            UpdateStudentInfo(manager);
                            break;
                        case "6":
                            DeleteStudentInfo(manager);
                            break;
                        case "7":
                            manager.DisplayStatistics();
                            break;
                        case "8":
                            manager.DisplayTopStudents();
                            break;
                        case "0":
                            running = false;
                            Console.WriteLine("\nขอบคุณที่ใช้งาน! ลาก่อน");
                            break;
                        default:
                            Console.WriteLine("\n✗ ตัวเลือกไม่ถูกต้อง กรุณาเลือกใหม่");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n✗ เกิดข้อผิดพลาด: {ex.Message}");
                }

                if (running)
                {
                    Console.WriteLine("\nกด Enter เพื่อดำเนินการต่อ...");
                    Console.ReadLine();
                }
            }
        }

        static void DisplayMenu()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║       ระบบจัดการข้อมูลนักเรียน .NET Core          ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine("  1. เพิ่มนักเรียนใหม่");
            Console.WriteLine("  2. แสดงรายชื่อนักเรียนทั้งหมด");
            Console.WriteLine("  3. ค้นหานักเรียนด้วย ID");
            Console.WriteLine("  4. ค้นหานักเรียนด้วยชื่อ");
            Console.WriteLine("  5. แก้ไขข้อมูลนักเรียน");
            Console.WriteLine("  6. ลบข้อมูลนักเรียน");
            Console.WriteLine("  7. แสดงสถิติ");
            Console.WriteLine("  8. แสดงนักเรียน Top 5");
            Console.WriteLine("  0. ออกจากโปรแกรม");
            Console.WriteLine();
            Console.Write("เลือกเมนู: ");
        }

        static void AddNewStudent(StudentManager manager)
        {
            Console.WriteLine("\n=== เพิ่มนักเรียนใหม่ ===");
            
            Console.Write("ชื่อ-นามสกุล: ");
            string? name = Console.ReadLine();
            
            Console.Write("อายุ: ");
            int age = int.Parse(Console.ReadLine() ?? "0");
            
            Console.Write("GPA (0.00-4.00): ");
            double gpa = double.Parse(Console.ReadLine() ?? "0");
            
            Console.Write("สาขา: ");
            string? major = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(major))
            {
                manager.AddStudent(name, age, gpa, major);
            }
            else
            {
                Console.WriteLine("\n✗ กรุณากรอกข้อมูลให้ครบถ้วน");
            }
        }

        static void SearchStudentById(StudentManager manager)
        {
            Console.Write("\nใส่ ID นักเรียน: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                manager.SearchStudent(id);
            }
            else
            {
                Console.WriteLine("✗ ID ไม่ถูกต้อง");
            }
        }

        static void SearchStudentByName(StudentManager manager)
        {
            Console.Write("\nใส่ชื่อนักเรียน: ");
            string? name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name))
            {
                manager.SearchByName(name);
            }
        }

        static void UpdateStudentInfo(StudentManager manager)
        {
            Console.Write("\nใส่ ID นักเรียนที่ต้องการแก้ไข: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                manager.UpdateStudent(id);
            }
            else
            {
                Console.WriteLine("✗ ID ไม่ถูกต้อง");
            }
        }

        static void DeleteStudentInfo(StudentManager manager)
        {
            Console.Write("\nใส่ ID นักเรียนที่ต้องการลบ: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                manager.DeleteStudent(id);
            }
            else
            {
                Console.WriteLine("✗ ID ไม่ถูกต้อง");
            }
        }
    }
}
```

## 4.3 การจัดการไฟล์ (File I/O)

### 4.3.1 การอ่านและเขียนไฟล์

สร้างไฟล์ `FileHelper.cs`:

```csharp
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;

namespace StudentManagement
{
    public class FileHelper
    {
        private const string DataFile = "students.json";

        // บันทึกข้อมูลลงไฟล์
        public static void SaveToFile<T>(List<T> data)
        {
            try
            {
                var options = new JsonSerializerOptions 
                { 
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                
                string json = JsonSerializer.Serialize(data, options);
                File.WriteAllText(DataFile, json);
                Console.WriteLine("✓ บันทึกข้อมูลเรียบร้อยแล้ว");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ เกิดข้อผิดพลาดในการบันทึก: {ex.Message}");
            }
        }

        // อ่านข้อมูลจากไฟล์
        public static List<T> LoadFromFile<T>()
        {
            try
            {
                if (File.Exists(DataFile))
                {
                    string json = File.ReadAllText(DataFile);
                    var data = JsonSerializer.Deserialize<List<T>>(json);
                    return data ?? new List<T>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ เกิดข้อผิดพลาดในการอ่าน: {ex.Message}");
            }
            
            return new List<T>();
        }

        // เขียนข้อความลงไฟล์ log
        public static void WriteLog(string message)
        {
            string logFile = $"log_{DateTime.Now:yyyyMMdd}.txt";
            string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            
            try
            {
                File.AppendAllText(logFile, logMessage + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ ไม่สามารถเขียน log: {ex.Message}");
            }
        }

        // อ่านไฟล์ทีละบรรทัด
        public static void ReadFileLineByLine(string filename)
        {
            if (!File.Exists(filename))
            {
                Console.WriteLine($"✗ ไม่พบไฟล์: {filename}");
                return;
            }

            try
            {
                using (StreamReader reader = new StreamReader(filename))
                {
                    string? line;
                    int lineNumber = 1;
                    
                    while ((line = reader.ReadLine()) != null)
                    {
                        Console.WriteLine($"{lineNumber}: {line}");
                        lineNumber++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ เกิดข้อผิดพลาด: {ex.Message}");
            }
        }

        // ตรวจสอบและสร้าง directory
        public static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Console.WriteLine($"✓ สร้างโฟลเดอร์: {path}");
            }
        }
    }
}
```

## 4.4 การทำงานกับ Arguments

### 4.4.1 Command Line Arguments

```csharp
// Program.cs - รับ arguments
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine($"จำนวน arguments: {args.Length}");
        
        for (int i = 0; i < args.Length; i++)
        {
            Console.WriteLine($"Argument {i}: {args[i]}");
        }

        // ตัวอย่างการใช้งาน
        if (args.Length > 0)
        {
            string command = args[0].ToLower();
            
            switch (command)
            {
                case "add":
                    if (args.Length >= 5)
                    {
                        // dotnet run -- add "สมชาย" 20 3.5 "CS"
                        Console.WriteLine($"เพิ่ม: {args[1]} {args[2]} {args[3]} {args[4]}");
                    }
                    break;
                case "list":
                    Console.WriteLine("แสดงรายการทั้งหมด");
                    break;
                case "help":
                    DisplayHelp();
                    break;
                default:
                    Console.WriteLine("คำสั่งไม่ถูกต้อง");
                    break;
            }
        }
    }

    static void DisplayHelp()
    {
        Console.WriteLine("\nคำสั่งที่ใช้ได้:");
        Console.WriteLine("  add [name] [age] [gpa] [major] - เพิ่มนักเรียน");
        Console.WriteLine("  list - แสดงรายการทั้งหมด");
        Console.WriteLine("  help - แสดงความช่วยเหลือ");
    }
}
```

### 4.4.2 การรันด้วย Arguments

```bash
# วิธีรัน
dotnet run -- add "สมชาย" 20 3.5 "CS"
dotnet run -- list
dotnet run -- help
```

## 4.5 ตัวอย่างโปรแกรมจริง

### 4.5.1 โปรแกรมจัดการรายรับรายจ่าย

```csharp
// ExpenseTracker.cs
public class Expense
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Category { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }

    public override string ToString()
    {
        return $"{Date:dd/MM/yyyy} | {Category,-15} | {Amount,10:C} | {Description}";
    }
}

public class ExpenseTracker
{
    private List<Expense> expenses = new List<Expense>();
    private int nextId = 1;

    public void AddExpense(string category, decimal amount, string description)
    {
        var expense = new Expense
        {
            Id = nextId++,
            Date = DateTime.Now,
            Category = category,
            Amount = amount,
            Description = description
        };
        expenses.Add(expense);
        Console.WriteLine("✓ บันทึกรายการเรียบร้อย");
    }

    public void DisplayAllExpenses()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                    รายการทั้งหมด                            ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
        
        foreach (var expense in expenses.OrderByDescending(e => e.Date))
        {
            Console.WriteLine(expense);
        }
        
        Console.WriteLine($"\nรวมทั้งหมด: {expenses.Sum(e => e.Amount):C}");
    }

    public void DisplaySummaryByCategory()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                  สรุปตามหมวดหมู่                            ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
        
        var summary = expenses
            .GroupBy(e => e.Category)
            .Select(g => new { Category = g.Key, Total = g.Sum(e => e.Amount) })
            .OrderByDescending(x => x.Total);

        foreach (var item in summary)
        {
            Console.WriteLine($"{item.Category,-20}: {item.Total,12:C}");
        }
    }

    public void DisplayMonthlyReport()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                  รายงานรายเดือน                             ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");

        var monthly = expenses
            .GroupBy(e => new { e.Date.Year, e.Date.Month })
            .Select(g => new 
            { 
                Period = $"{g.Key.Month:D2}/{g.Key.Year}",
                Total = g.Sum(e => e.Amount),
                Count = g.Count()
            })
            .OrderByDescending(x => x.Period);

        foreach (var month in monthly)
        {
            Console.WriteLine($"{month.Period}: {month.Total,12:C} ({month.Count} รายการ)");
        }
    }
}
```

### 4.5.2 โปรแกรม To-Do List

```csharp
// TodoList.cs
public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public Priority Priority { get; set; }

    public string GetStatus()
    {
        return IsCompleted ? "✓ เสร็จแล้ว" : "○ ยังไม่เสร็จ";
    }

    public override string ToString()
    {
        string priorityIcon = Priority switch
        {
            Priority.High => "🔴",
            Priority.Medium => "🟡",
            Priority.Low => "🟢",
            _ => "⚪"
        };

        return $"{priorityIcon} [{Id}] {GetStatus()} {Title}";
    }
}

public enum Priority
{
    Low,
    Medium,
    High
}

public class TodoListManager
{
    private List<TodoItem> tasks = new List<TodoItem>();
    private int nextId = 1;

    public void AddTask(string title, string description, Priority priority)
    {
        var task = new TodoItem
        {
            Id = nextId++,
            Title = title,
            Description = description,
            Priority = priority,
            IsCompleted = false,
            CreatedDate = DateTime.Now
        };
        tasks.Add(task);
        Console.WriteLine($"✓ เพิ่มงาน: {title}");
    }

    public void DisplayAllTasks()
    {
        Console.WriteLine("\n╔════════════════════════════════════════════════════╗");
        Console.WriteLine("║              รายการงานทั้งหมด                      ║");
        Console.WriteLine("╚════════════════════════════════════════════════════╝");

        var pendingTasks = tasks.Where(t => !t.IsCompleted).OrderByDescending(t => t.Priority);
        var completedTasks = tasks.Where(t => t.IsCompleted);

        Console.WriteLine("\n--- งานที่ยังไม่เสร็จ ---");
        foreach (var task in pendingTasks)
        {
            Console.WriteLine(task);
        }

        Console.WriteLine("\n--- งานที่เสร็จแล้ว ---");
        foreach (var task in completedTasks)
        {
            Console.WriteLine(task);
        }

        Console.WriteLine($"\nรวม: {pendingTasks.Count()} งานที่ค้าง, {completedTasks.Count()} งานเสร็จแล้ว");
    }

    public void CompleteTask(int id)
    {
        var task = tasks.FirstOrDefault(t => t.Id == id);
        if (task != null)
        {
            task.IsCompleted = true;
            task.CompletedDate = DateTime.Now;
            Console.WriteLine($"✓ ทำงาน '{task.Title}' เสร็จแล้ว!");
        }
        else
        {
            Console.WriteLine($"✗ ไม่พบงาน ID: {id}");
        }
    }
}
```

## 4.6 สรุป

ในบทนี้เราได้เรียนรู้:

✅ การสร้างและจัดการ Console Application
✅ การสร้างระบบจัดการข้อมูลนักเรียนแบบสมบูรณ์
✅ การอ่านและเขียนไฟล์
✅ การทำงานกับ Command Line Arguments
✅ ตัวอย่างโปรแกรมจริง (Expense Tracker, Todo List)

## 📚 แบบฝึกหัด

1. เพิ่มฟังก์ชัน Export ข้อมูลเป็น CSV ในระบบจัดการนักเรียน
2. สร้างระบบ Login สำหรับผู้ใช้งาน
3. เพิ่มการ Validate ข้อมูลก่อนบันทึก
4. สร้างระบบจัดการหนังสือในห้องสมุด
5. พัฒนาโปรแกรม Budget Planner

### โจทย์ท้าทาย: Library Management System

สร้างระบบจัดการหนังสือที่มีฟีเจอร์:
- เพิ่ม/ลบ/แก้ไข หนังสือ
- ระบบยืม-คืนหนังสือ
- ค้นหาหนังสือ (ชื่อ, ผู้แต่ง, ISBN)
- สถิติการยืม
- บันทึกข้อมูลลงไฟล์

---

**ก่อนหน้า:** [บทที่ 3 - พื้นฐาน C#](../Chapter03-CSharpFundamentals/README.md)
**ต่อไป:** [บทที่ 5 - ASP.NET Core Web Applications](../Chapter05-WebApplications/README.md)
