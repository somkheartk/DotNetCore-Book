// ตัวอย่างโปรแกรมเครื่องคิดเลข (Calculator)
// สำหรับแบบฝึกหัดบทที่ 2

using System;

namespace CalculatorApp
{
    class Program
    {
        static void Main(string[] args)
        {
            bool continueCalculating = true;

            Console.WriteLine("╔═══════════════════════════════════╗");
            Console.WriteLine("║     เครื่องคิดเลข .NET Core     ║");
            Console.WriteLine("╚═══════════════════════════════════╝");
            Console.WriteLine();

            while (continueCalculating)
            {
                try
                {
                    // รับตัวเลขที่ 1
                    Console.Write("ใส่ตัวเลขที่ 1: ");
                    double num1 = Convert.ToDouble(Console.ReadLine());

                    // รับตัวเลขที่ 2
                    Console.Write("ใส่ตัวเลขที่ 2: ");
                    double num2 = Convert.ToDouble(Console.ReadLine());

                    // แสดงตัวเลือก operation
                    Console.WriteLine();
                    Console.WriteLine("เลือก operation:");
                    Console.WriteLine("1. บวก (+)");
                    Console.WriteLine("2. ลบ (-)");
                    Console.WriteLine("3. คูณ (*)");
                    Console.WriteLine("4. หาร (/)");
                    Console.Write("เลือก (1-4): ");
                    
                    string? choice = Console.ReadLine();
                    double result = 0;
                    string operation = "";

                    // คำนวณตาม operation ที่เลือก
                    switch (choice)
                    {
                        case "1":
                            result = num1 + num2;
                            operation = "+";
                            break;
                        case "2":
                            result = num1 - num2;
                            operation = "-";
                            break;
                        case "3":
                            result = num1 * num2;
                            operation = "*";
                            break;
                        case "4":
                            if (num2 != 0)
                            {
                                result = num1 / num2;
                                operation = "/";
                            }
                            else
                            {
                                Console.WriteLine();
                                Console.WriteLine("ไม่สามารถหารด้วย 0 ได้!");
                                continue;
                            }
                            break;
                        default:
                            Console.WriteLine();
                            Console.WriteLine("ตัวเลือกไม่ถูกต้อง!");
                            continue;
                    }

                    // แสดงผลลัพธ์
                    Console.WriteLine();
                    Console.WriteLine("═══════════════════════════════════");
                    Console.WriteLine($"ผลลัพธ์: {num1} {operation} {num2} = {result}");
                    Console.WriteLine("═══════════════════════════════════");
                }
                catch (FormatException)
                {
                    Console.WriteLine();
                    Console.WriteLine("กรุณาใส่ตัวเลขที่ถูกต้อง!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine($"เกิดข้อผิดพลาด: {ex.Message}");
                }

                // ถามว่าต้องการคำนวณต่อหรือไม่
                Console.WriteLine();
                Console.Write("คำนวณต่อไหม? (y/n): ");
                string? answer = Console.ReadLine()?.ToLower();
                continueCalculating = (answer == "y" || answer == "yes");
                Console.WriteLine();
            }

            Console.WriteLine("ขอบคุณที่ใช้งานเครื่องคิดเลข!");
        }
    }
}
