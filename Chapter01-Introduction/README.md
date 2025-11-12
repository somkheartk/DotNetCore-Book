# บทที่ 1: แนะนำ .NET Core

## 1.1 .NET Core คืออะไร

.NET Core เป็น Framework แบบ Open Source สำหรับการพัฒนาแอปพลิเคชันแบบ Cross-platform ที่พัฒนาโดย Microsoft สามารถรันได้บนระบบปฏิบัติการ Windows, Linux, และ macOS

### คุณสมบัติสำคัญ:

1. **Cross-platform** - รองรับหลายระบบปฏิบัติการ
2. **Open Source** - โค้ดเปิดให้ชุมชนพัฒนาต่อยอด
3. **High Performance** - ประสิทธิภาพสูง
4. **Modular** - สามารถเลือกใช้เฉพาะส่วนที่ต้องการ
5. **Modern** - รองรับเทคโนโลยีและ Pattern ใหม่ๆ

## 1.2 ประวัติและวิวัฒนาการ

### Timeline:

- **2002**: .NET Framework 1.0 เปิดตัว
- **2016**: .NET Core 1.0 เปิดตัว
- **2017**: .NET Core 2.0 เพิ่มความสามารถมากขึ้น
- **2018**: .NET Core 2.1 LTS (Long Term Support)
- **2019**: .NET Core 3.0 และ 3.1 LTS
- **2020**: .NET 5 (รวม .NET Core และ .NET Framework)
- **2021**: .NET 6 LTS
- **2022**: .NET 7
- **2023**: .NET 8 LTS

### Evolution Diagram:

```
.NET Framework (Windows Only)
        |
        ↓
    .NET Core (Cross-platform)
        |
        ↓
    .NET 5+ (Unified Platform)
```

## 1.3 ข้อดีของ .NET Core

### 1.3.1 Performance (ประสิทธิภาพ)

```csharp
// ตัวอย่าง: .NET Core มี Performance สูงกว่า
// Benchmark results แสดงว่า .NET Core สามารถจัดการ
// requests ได้มากกว่า Node.js และ Java Spring Boot
```

**ข้อมูล Benchmark:**
- สามารถจัดการ HTTP requests ได้ 7+ million requests/second
- Memory usage ต่ำกว่า
- Startup time เร็วกว่า

### 1.3.2 Cross-platform Development

```bash
# พัฒนาบน Windows
dotnet build
dotnet run

# Deploy บน Linux
dotnet MyApp.dll

# รันบน macOS
dotnet MyApp.dll
```

### 1.3.3 Microservices และ Cloud-ready

.NET Core ออกแบบมาเพื่อรองรับ:
- Docker containers
- Kubernetes orchestration
- Cloud platforms (Azure, AWS, Google Cloud)
- Microservices architecture

### 1.3.4 Modern Development

- Supports C# 10/11/12 (ล่าสุด)
- Async/await programming
- LINQ (Language Integrated Query)
- Dependency Injection built-in
- Configuration flexibility

## 1.4 เปรียบเทียบ .NET Framework vs .NET Core vs .NET 5+

### ตารางเปรียบเทียบ:

| Feature | .NET Framework | .NET Core | .NET 5+ |
|---------|---------------|-----------|---------|
| Platform | Windows Only | Cross-platform | Cross-platform |
| Open Source | ❌ | ✅ | ✅ |
| Performance | Good | Excellent | Excellent |
| WinForms/WPF | ✅ | ❌ (Core 3.0+: ✅) | ✅ |
| ASP.NET Core | ❌ | ✅ | ✅ |
| Side-by-side versions | ❌ | ✅ | ✅ |
| Microservices | Limited | ✅ | ✅ |
| Docker Support | Limited | ✅ | ✅ |
| New Features | ❌ | ✅ | ✅ |
| Long-term Support | Legacy | Selected versions | Every 2 years |

### 1.4.1 เมื่อไหร่ควรใช้อะไร?

**ใช้ .NET Framework เมื่อ:**
- มี legacy code ที่ต้อง maintain
- ใช้ Windows-specific APIs เยอะ
- ใช้ technologies ที่ไม่ support .NET Core

**ใช้ .NET Core / .NET 5+ เมื่อ:**
- เริ่มโปรเจคใหม่
- ต้องการ cross-platform support
- ต้องการ performance สูง
- พัฒนา microservices
- Deploy บน containers

## 1.5 Application Types ที่รองรับ

### 1.5.1 Console Applications
```csharp
// Program.cs
Console.WriteLine("Hello .NET Core!");
```

### 1.5.2 Web Applications (ASP.NET Core)
- MVC Applications
- Razor Pages
- Blazor (WebAssembly & Server)
- Web APIs

### 1.5.3 Desktop Applications
- WPF (Windows Presentation Foundation)
- WinForms
- MAUI (Multi-platform App UI)

### 1.5.4 Cloud & Microservices
- Azure Functions
- Docker Containers
- gRPC Services

### 1.5.5 Mobile Applications
- .NET MAUI
- Xamarin (legacy)

### 1.5.6 IoT & AI/ML
- IoT Applications
- ML.NET for Machine Learning

## 1.6 .NET Core Architecture

### High-level Architecture:

```
┌─────────────────────────────────────┐
│     Application Code (C#, F#)      │
├─────────────────────────────────────┤
│        .NET Libraries (BCL)         │
├─────────────────────────────────────┤
│         Runtime (CoreCLR)           │
├─────────────────────────────────────┤
│    Operating System (Win/Linux/Mac) │
└─────────────────────────────────────┘
```

### Components:

1. **CoreCLR**: Runtime engine
2. **CoreFX**: Base Class Libraries
3. **ASP.NET Core**: Web framework
4. **Entity Framework Core**: ORM
5. **CLI Tools**: Command-line interface

## 1.7 ตัวอย่าง Code แรก

### สร้าง Hello World Application:

```csharp
// Program.cs
namespace HelloDotNetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("สวัสดี .NET Core!");
            Console.WriteLine($"เวอร์ชันปัจจุบัน: {Environment.Version}");
            Console.WriteLine($"ระบบปฏิบัติการ: {Environment.OSVersion}");
            
            // แสดงข้อมูลระบบ
            Console.WriteLine($"\nรันบน: {GetOSPlatform()}");
        }
        
        static string GetOSPlatform()
        {
            if (OperatingSystem.IsWindows())
                return "Windows";
            else if (OperatingSystem.IsLinux())
                return "Linux";
            else if (OperatingSystem.IsMacOS())
                return "macOS";
            else
                return "Unknown";
        }
    }
}
```

### Output ตัวอย่าง:
```
สวัสดี .NET Core!
เวอร์ชันปัจจุบัน: 8.0.0
ระบบปฏิบัติการ: Microsoft Windows NT 10.0.19045.0

รันบน: Windows
```

## 1.8 ระบบนิเวศน์ของ .NET (Ecosystem)

### Development Tools:
- **Visual Studio**: Full-featured IDE (Windows/Mac)
- **Visual Studio Code**: Lightweight editor (Cross-platform)
- **JetBrains Rider**: Alternative IDE
- **Command Line Tools**: dotnet CLI

### Package Management:
- **NuGet**: Package manager
- ที่เก็บ packages มากกว่า 300,000 packages

### Community & Resources:
- GitHub: https://github.com/dotnet
- Documentation: https://docs.microsoft.com/dotnet
- .NET Foundation: https://dotnetfoundation.org

## 1.9 การเลือกเวอร์ชัน .NET

### LTS vs Current:

**LTS (Long Term Support):**
- รองรับนาน 3 ปี
- สำหรับ production
- Stable และ reliable
- ตัวอย่าง: .NET 6, .NET 8

**Current (Standard Term Support):**
- รองรับ 18 เดือน
- Features ใหม่เร็วกว่า
- สำหรับ early adopters
- ตัวอย่าง: .NET 7

### แนะนำ:
- **Production Apps**: ใช้ LTS version
- **Learning/Testing**: ใช้ Current version ล่าสุด

## 1.10 สรุป

ในบทนี้เราได้เรียนรู้:

✅ .NET Core คืออะไรและมีคุณสมบัติอะไรบ้าง
✅ ประวัติความเป็นมาและวิวัฒนาการ
✅ ข้อดีและข้อเด่นของ .NET Core
✅ ความแตกต่างระหว่าง .NET Framework, .NET Core และ .NET 5+
✅ ประเภทของ applications ที่พัฒนาได้
✅ Architecture และ components
✅ ระบบนิเวศน์ของ .NET

## 📚 แบบฝึกหัด

1. .NET Core สามารถรันได้บนระบบปฏิบัติการอะไรบ้าง?
2. LTS ย่อมาจากอะไร และมีความสำคัญอย่างไร?
3. ข้อดีหลักของ .NET Core เมื่อเทียบกับ .NET Framework คือ?
4. .NET 6 และ .NET 8 ต่างกันอย่างไร?
5. Application types ใดบ้างที่สามารถพัฒนาด้วย .NET Core?

## 🔗 เอกสารอ้างอิง

- [Official .NET Documentation](https://docs.microsoft.com/dotnet)
- [.NET GitHub Repository](https://github.com/dotnet)
- [.NET Blog](https://devblogs.microsoft.com/dotnet/)
- [.NET Foundation](https://dotnetfoundation.org)

---

**ต่อไป:** [บทที่ 2 - การติดตั้งและเริ่มต้นใช้งาน](../Chapter02-GettingStarted/README.md)
