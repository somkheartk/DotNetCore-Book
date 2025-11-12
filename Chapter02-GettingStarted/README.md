# บทที่ 2: การติดตั้งและเริ่มต้นใช้งาน

## 2.1 การติดตั้ง .NET SDK

### 2.1.1 Windows

**วิธีที่ 1: ดาวน์โหลดจากเว็บไซต์**

1. ไปที่ https://dotnet.microsoft.com/download
2. เลือก .NET SDK เวอร์ชันล่าสุด (แนะนำ LTS)
3. ดาวน์โหลด Installer สำหรับ Windows
4. รัน Installer และทำตามขั้นตอน

**วิธีที่ 2: ใช้ Windows Package Manager**

```powershell
# ใช้ winget
winget install Microsoft.DotNet.SDK.8

# หรือใช้ Chocolatey
choco install dotnet-sdk
```

### 2.1.2 Linux (Ubuntu/Debian)

```bash
# เพิ่ม Microsoft package repository
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# Update package list
sudo apt-get update

# ติดตั้ง .NET SDK
sudo apt-get install -y dotnet-sdk-8.0
```

### 2.1.3 macOS

**วิธีที่ 1: ดาวน์โหลด Installer**

1. ไปที่ https://dotnet.microsoft.com/download
2. ดาวน์โหลด .NET SDK สำหรับ macOS
3. รัน .pkg file และทำตามขั้นตอน

**วิธีที่ 2: ใช้ Homebrew**

```bash
brew install --cask dotnet-sdk
```

## 2.2 การตรวจสอบการติดตั้ง

### ตรวจสอบเวอร์ชัน:

```bash
dotnet --version
```

**Output ตัวอย่าง:**
```
8.0.100
```

### ตรวจสอบข้อมูลทั้งหมด:

```bash
dotnet --info
```

**Output ตัวอย่าง:**
```
.NET SDK:
 Version:   8.0.100
 Commit:    abc12345

Runtime Environment:
 OS Name:     Windows
 OS Version:  10.0.19045
 OS Platform: Windows
 RID:         win10-x64
 Base Path:   C:\Program Files\dotnet\sdk\8.0.100\

.NET runtimes installed:
  Microsoft.AspNetCore.App 8.0.0 [C:\Program Files\dotnet\shared\Microsoft.AspNetCore.App]
  Microsoft.NETCore.App 8.0.0 [C:\Program Files\dotnet\shared\Microsoft.NETCore.App]
```

### แสดง SDKs ที่ติดตั้ง:

```bash
dotnet --list-sdks
```

### แสดง Runtimes ที่ติดตั้ง:

```bash
dotnet --list-runtimes
```

## 2.3 การติดตั้ง Visual Studio Code

### 2.3.1 ดาวน์โหลดและติดตั้ง

1. ไปที่ https://code.visualstudio.com
2. ดาวน์โหลดตามระบบปฏิบัติการ
3. ติดตั้งตามขั้นตอน

### 2.3.2 Extensions ที่จำเป็น

ติดตั้ง Extensions ต่อไปนี้:

1. **C# Dev Kit** (Microsoft)
   - รองรับ C# IntelliSense
   - Debugging
   - Code navigation

2. **C#** (Microsoft)
   - Syntax highlighting
   - Code completion

3. **.NET Install Tool** (Microsoft)
   - จัดการ .NET SDKs

4. **NuGet Package Manager** (jmrog)
   - จัดการ NuGet packages

### ติดตั้ง Extensions ผ่าน Command Line:

```bash
code --install-extension ms-dotnettools.csdevkit
code --install-extension ms-dotnettools.csharp
code --install-extension jmrog.vscode-nuget-package-manager
```

## 2.4 การติดตั้ง Visual Studio

### Visual Studio 2022 Community (ฟรี)

1. ดาวน์โหลดจาก https://visualstudio.microsoft.com
2. เลือก Workloads:
   - **ASP.NET and web development**
   - **.NET desktop development**
   - **.NET Core cross-platform development**

### Workloads แนะนำ:
- ASP.NET and web development
- .NET desktop development
- Data storage and processing (สำหรับ database)

## 2.5 CLI Commands พื้นฐาน

### 2.5.1 dotnet new

สร้างโปรเจคใหม่:

```bash
# แสดง templates ทั้งหมด
dotnet new list

# สร้าง Console App
dotnet new console -n MyFirstApp

# สร้าง Web API
dotnet new webapi -n MyWebApi

# สร้าง MVC Web App
dotnet new mvc -n MyWebApp

# สร้าง Class Library
dotnet new classlib -n MyLibrary

# สร้าง Solution file
dotnet new sln -n MySolution
```

### 2.5.2 dotnet build

Build โปรเจค:

```bash
# Build โปรเจคปัจจุบัน
dotnet build

# Build ด้วย configuration
dotnet build --configuration Release

# Build และแสดง verbose output
dotnet build --verbosity detailed
```

### 2.5.3 dotnet run

รันแอปพลิเคชัน:

```bash
# รันโปรเจคปัจจุบัน
dotnet run

# รันพร้อม arguments
dotnet run -- arg1 arg2

# รันด้วย configuration
dotnet run --configuration Release
```

### 2.5.4 dotnet test

รัน tests:

```bash
# รัน tests ทั้งหมด
dotnet test

# รันพร้อม code coverage
dotnet test --collect:"XPlat Code Coverage"
```

### 2.5.5 dotnet publish

สร้างไฟล์สำหรับ deployment:

```bash
# Publish โปรเจค
dotnet publish -c Release -o ./publish

# Publish แบบ self-contained
dotnet publish -c Release -r win-x64 --self-contained true

# Publish แบบ single file
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

### 2.5.6 dotnet add

เพิ่ม package หรือ reference:

```bash
# เพิ่ม NuGet package
dotnet add package Newtonsoft.Json

# เพิ่ม package เวอร์ชันเฉพาะ
dotnet add package Newtonsoft.Json --version 13.0.3

# เพิ่ม project reference
dotnet add reference ../MyLibrary/MyLibrary.csproj
```

### 2.5.7 dotnet restore

Restore dependencies:

```bash
dotnet restore
```

## 2.6 โครงสร้างโปรเจค .NET Core

### 2.6.1 Console Application

```
MyConsoleApp/
├── Program.cs              # Entry point
├── MyConsoleApp.csproj    # Project file
├── bin/                   # Build output
│   └── Debug/
│       └── net8.0/
└── obj/                   # Intermediate files
```

### 2.6.2 ไฟล์ .csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

**อธิบาย:**
- `OutputType`: ประเภทของ output (Exe, Library)
- `TargetFramework`: เวอร์ชัน .NET ที่ใช้
- `ImplicitUsings`: เปิดใช้ global usings
- `Nullable`: เปิดใช้ nullable reference types

### 2.6.3 Program.cs (Top-level statements)

```csharp
// Program.cs - .NET 6+ style
Console.WriteLine("Hello, World!");
```

หรือแบบเต็ม:

```csharp
// Program.cs - Traditional style
namespace MyConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
    }
}
```

## 2.7 สร้างโปรเจคแรกของคุณ

### ขั้นตอนที่ 1: สร้างโปรเจค

```bash
# สร้าง directory
mkdir MyFirstDotNetApp
cd MyFirstDotNetApp

# สร้างโปรเจค Console
dotnet new console
```

### ขั้นตอนที่ 2: แก้ไข Program.cs

```csharp
// Program.cs
using System;

Console.WriteLine("=== โปรแกรมแรกของฉัน ===");
Console.WriteLine();

// รับชื่อจากผู้ใช้
Console.Write("กรุณาใส่ชื่อของคุณ: ");
string? name = Console.ReadLine();

Console.Write("กรุณาใส่อายุของคุณ: ");
string? ageInput = Console.ReadLine();

if (int.TryParse(ageInput, out int age))
{
    Console.WriteLine();
    Console.WriteLine($"สวัสดี {name}!");
    Console.WriteLine($"คุณอายุ {age} ปี");
    
    // คำนวณปีเกิด (โดยประมาณ)
    int currentYear = DateTime.Now.Year;
    int birthYear = currentYear - age;
    Console.WriteLine($"คุณเกิดประมาณปี พ.ศ. {birthYear + 543}");
}
else
{
    Console.WriteLine("อายุไม่ถูกต้อง!");
}

Console.WriteLine();
Console.WriteLine("กด Enter เพื่อออก...");
Console.ReadLine();
```

### ขั้นตอนที่ 3: Build และ Run

```bash
# Build โปรเจค
dotnet build

# รันโปรแกรม
dotnet run
```

**Output ตัวอย่าง:**
```
=== โปรแกรมแรกของฉัน ===

กรุณาใส่ชื่อของคุณ: สมชาย
กรุณาใส่อายุของคุณ: 25

สวัสดี สมชาย!
คุณอายุ 25 ปี
คุณเกิดประมาณปี พ.ศ. 2542

กด Enter เพื่อออก...
```

## 2.8 การจัดการ NuGet Packages

### 2.8.1 ค้นหา Packages

```bash
# ค้นหา package
dotnet nuget search Newtonsoft.Json
```

หรือค้นหาที่ https://www.nuget.org

### 2.8.2 เพิ่ม Package

```bash
# เพิ่ม package
dotnet add package Newtonsoft.Json

# เพิ่มเวอร์ชันเฉพาะ
dotnet add package Newtonsoft.Json --version 13.0.3
```

### 2.8.3 ลบ Package

```bash
dotnet remove package Newtonsoft.Json
```

### 2.8.4 Update Package

```bash
# List outdated packages
dotnet list package --outdated

# Update package
dotnet add package Newtonsoft.Json
```

### 2.8.5 ตัวอย่างการใช้ NuGet Package

```csharp
// Program.cs
using Newtonsoft.Json;

var person = new 
{
    Name = "สมชาย",
    Age = 25,
    City = "กรุงเทพฯ"
};

// Serialize to JSON
string json = JsonConvert.SerializeObject(person, Formatting.Indented);
Console.WriteLine("JSON Output:");
Console.WriteLine(json);

// Deserialize from JSON
var deserializedPerson = JsonConvert.DeserializeObject(json);
Console.WriteLine("\nDeserialized:");
Console.WriteLine(deserializedPerson);
```

## 2.9 Solution และ Multiple Projects

### สร้าง Solution:

```bash
# สร้าง solution
dotnet new sln -n MyCompleteSolution

# สร้าง projects
dotnet new console -n MyConsoleApp
dotnet new classlib -n MyLibrary
dotnet new xunit -n MyTests

# เพิ่ม projects เข้า solution
dotnet sln add MyConsoleApp/MyConsoleApp.csproj
dotnet sln add MyLibrary/MyLibrary.csproj
dotnet sln add MyTests/MyTests.csproj

# เพิ่ม project reference
cd MyConsoleApp
dotnet add reference ../MyLibrary/MyLibrary.csproj
```

### โครงสร้าง Solution:

```
MyCompleteSolution/
├── MyCompleteSolution.sln
├── MyConsoleApp/
│   ├── Program.cs
│   └── MyConsoleApp.csproj
├── MyLibrary/
│   ├── Class1.cs
│   └── MyLibrary.csproj
└── MyTests/
    ├── UnitTest1.cs
    └── MyTests.csproj
```

## 2.10 Configuration Files

### 2.10.1 global.json

ระบุเวอร์ชัน SDK:

```json
{
  "sdk": {
    "version": "8.0.100",
    "rollForward": "latestMinor"
  }
}
```

### 2.10.2 .editorconfig

กำหนด coding style:

```ini
root = true

[*.cs]
indent_style = space
indent_size = 4
charset = utf-8
trim_trailing_whitespace = true
insert_final_newline = true
```

### 2.10.3 .gitignore

สำหรับ Git:

```
## .NET
bin/
obj/
*.user
*.suo
.vs/
```

## 2.11 Hot Reload

.NET 6+ รองรับ Hot Reload:

```bash
# รันด้วย Hot Reload (watch mode)
dotnet watch run
```

เมื่อแก้ไขโค้ด โปรแกรมจะ reload อัตโนมัติ

## 2.12 Troubleshooting

### ปัญหาที่พบบ่อย:

**1. dotnet command not found**
```bash
# ตรวจสอบ PATH
echo $PATH  # Linux/Mac
echo %PATH%  # Windows

# เพิ่ม PATH (ถ้าจำเป็น)
export PATH="$PATH:/usr/local/share/dotnet"  # Mac
```

**2. SDK not found**
```bash
# ติดตั้ง SDK ใหม่
dotnet --list-sdks
```

**3. Build errors**
```bash
# Clean และ rebuild
dotnet clean
dotnet restore
dotnet build
```

## 2.13 สรุป

ในบทนี้เราได้เรียนรู้:

✅ วิธีการติดตั้ง .NET SDK บนระบบต่างๆ
✅ การตรวจสอบการติดตั้ง
✅ การติดตั้ง Visual Studio Code และ Extensions
✅ CLI commands พื้นฐาน (dotnet new, build, run, etc.)
✅ โครงสร้างโปรเจค .NET Core
✅ การสร้างและรันโปรเจคแรก
✅ การจัดการ NuGet packages
✅ การทำงานกับ Solutions
✅ Configuration files

## 📚 แบบฝึกหัด

1. ติดตั้ง .NET SDK และตรวจสอบเวอร์ชัน
2. สร้าง Console Application ที่รับ input 2 ตัวเลขและแสดงผลบวก ลบ คูณ หาร
3. สร้าง Solution ที่มี 2 projects: Console App และ Class Library
4. เพิ่ม NuGet package "Newtonsoft.Json" และทดลองใช้งาน
5. สร้าง .gitignore file สำหรับโปรเจค .NET

### โจทย์: สร้างเครื่องคิดเลข

```csharp
// สร้างโปรแกรมเครื่องคิดเลขแบบง่าย
// - รับตัวเลข 2 ตัว
// - เลือก operation (+, -, *, /)
// - แสดงผลลัพธ์
// - วนลูปจนกว่าผู้ใช้จะเลือกออก
```

## 🔗 เอกสารอ้างอิง

- [Install .NET](https://dotnet.microsoft.com/download)
- [.NET CLI Overview](https://docs.microsoft.com/dotnet/core/tools/)
- [NuGet Documentation](https://docs.microsoft.com/nuget/)
- [Visual Studio Code](https://code.visualstudio.com)

---

**ก่อนหน้า:** [บทที่ 1 - แนะนำ .NET Core](../Chapter01-Introduction/README.md)
**ต่อไป:** [บทที่ 3 - พื้นฐาน C#](../Chapter03-CSharpFundamentals/README.md)
