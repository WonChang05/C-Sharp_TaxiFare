using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace taxiFare
{
    class Program
    {
        // 判斷該縣市對應該時段是屬於白天還是晚上
        private static void setDictionary(Dictionary<int,int> dc,string[,] nh,int cityPos,string inputTime) {
            int dayStart = int.Parse(nh[cityPos, 1].Replace(":", ""));
            int dayEnd = int.Parse(nh[cityPos, 0].Replace(":", ""));
            if (int.Parse(inputTime) >= dayStart && int.Parse(inputTime) <= dayEnd)
            {
                dc.Add(cityPos, 0);//白天
            }
            else
            {
                dc.Add(cityPos, 1);//晚上
            }
        }

        private static string calculate(int cityPos,int dayOrNight, int meter,int sec,int[,,] bigData) {
            string result = "";

            //先計算公里數的差異 (要輸入的公里-起始公里)
            int mile = 0;
            if (meter > bigData[dayOrNight,0, cityPos])
            {
                mile = (meter - bigData[dayOrNight, 0, cityPos]) / bigData[dayOrNight, 2, cityPos];
            }
            // 再計算停車時間的差異
            int traffic = 0;
            if (sec > bigData[dayOrNight, 4, cityPos])
            {
                traffic = sec / bigData[dayOrNight, 4, cityPos];
            }

            //計算起始金額
            int baseAmt = dayOrNight == 0 ? bigData[dayOrNight, 1, cityPos] : bigData[0, 1, cityPos] + bigData[1, 1, cityPos];

            // 計算最後的金額
            int tot = baseAmt + (mile * bigData[dayOrNight, 3, cityPos]) + (traffic* bigData[dayOrNight, 5, cityPos]);

            result = $"{tot}元\t 【計算公式: {baseAmt}+{(mile * bigData[dayOrNight, 3, cityPos])}+{(traffic * bigData[dayOrNight, 5, cityPos])}】";
            return result;
        }

        static void Main(string[] args)
        {
            string[] city= { "雙北", "基隆", "宜蘭", "桃園", "新竹", "苗栗", "台中", "彰化", "南投", "嘉義", "台南", "高雄", "屏東", "花蓮", "台東", "雲林" };

            string[,] NightHour = { {"23:00","06:00"},{"23:00","06:00"},{"23:00","06:00"},{"24:00","00:00"},{"23:00","06:00"},
                                    {"23:00","06:00"},{"23:00","06:00"},{"22:00","06:00"},{"22:00","06:00"},{"24:00","00:00"},
                                    {"23:00","06:00"},{"23:00","06:00"},{"23:00","06:00"},{"22:00","06:00"},{"22:00","06:00"},
                                    {"23:00","06:00"}};

            //int[,,] bigData = new int[2, 6, 16];
            //int[,,] bigData = new int[白天或者晚上, 6種資料型態, 16個縣市];
            int[,,] bigData = { {
                    // 白天
                        //起始里程 0
                    { 1250, 1250, 1250, 1250, 1250, 1250, 1500, 1500, 1500, 1250, 1500, 1500, 1500, 1000, 1000, 1500 },
                        //起跳金額 1
                    { 70, 70, 120, 95, 100, 100, 85, 100, 75, 100, 85, 85, 100, 100, 100, 100 },
                        //接續計算里程 2
                    { 200, 200, 300, 250, 250, 250, 200, 250, 250, 250, 250, 250, 250, 230, 230, 300 },
                        //接續計算金額 3
                    { 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 },
                        //延滯計費基準 4
                    { 80, 80, 120, 150, 180, 180, 120, 180, 180, 150, 180, 180, 180, 120, 180, 180 },
                        //延滯計費基準 5
                    { 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 } }, {
                    // 晚上
                    { 1250, 1250, 1250, 1250, 1250, 1250, 1500, 1250, 1250, 1250, 1250, 1250, 1250, 834, 834, 1200 },
                        //夜間加乘 1
                    { 20, 20, 0, 0, 0, 0, 20, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 200, 200, 250, 250, 250, 250, 200, 200, 200, 250, 200, 200, 200, 192, 192, 250 },
                    { 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 },
                    { 80, 80, 100, 150, 180, 180, 120, 150, 150, 150, 150, 150, 150, 100, 150, 180 },
                    { 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 }
                    } };

            //列出所有縣市
            string temp = "";
            for (int i = 1; i <= 4; i++)
            {
                for (int j =(i-1)*4; j < i*4; j++)
                {
                    temp += $"{(j+1).ToString().PadLeft(2, '0')}.{city[j]}\t";
                }
                temp += "\n";
            }
            Console.WriteLine(temp);

            Console.Write("請輸入想要查詢的縣市 (1-16，0表示全部): ");
            string inputCity = Console.ReadLine();
            Console.WriteLine();

            Console.Write("請輸入乘車時間: ");
            string inputTime = Console.ReadLine();
            Console.WriteLine();

            Console.Write("預估行車停留時間: (分鐘) ");
            string inputMinus = Console.ReadLine();
            Console.WriteLine();

            Console.Write("請輸入里程 (公里): ");
            string inputDistance = Console.ReadLine();
            Console.WriteLine();

            Console.WriteLine("試算乘車費用如下:\n");
            temp = "";

            // 將輸入的數值轉換變成可以計算的數字
            int sec = int.Parse(inputMinus) * 60; // 延滯以秒計
            int meter = int.Parse(inputDistance) * 1000; // 乘車以公尺計

            //計算是白天或者黑夜
            //     cityPos,dayOrNight
            Dictionary<int, int> dc = new Dictionary<int, int>();
            if (inputCity != "0")
            {
                setDictionary(dc, NightHour, int.Parse(inputCity)-1, inputTime);
            }
            else
            {   //全部縣市
                for (int i = 0; i < city.Length; i++)
                {
                    setDictionary(dc, NightHour, i, inputTime);
                }
            }

            //計算車資
            foreach (KeyValuePair<int,int> item in dc)
            {
                temp += $"{(item.Key+1).ToString().PadLeft(2,'0')}.{city[item.Key]}\t";
                temp += item.Value == 0 ? "日間車資: ":"夜間車資: ";
                temp += calculate(item.Key,item.Value,meter,sec,bigData);
                temp += "\n";
            }
            Console.WriteLine(temp);
        }
    }
}
