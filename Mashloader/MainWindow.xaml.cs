using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Reflection;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.ObjectModel;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {

        IWebDriver driver = null;

        public MainWindow()
        {
            InitializeComponent();
            // Webドライバーのインスタンス
            driver = new ChromeDriver(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
        }

        private void Login_botton(object sender, RoutedEventArgs e)
        {
 

            // ログインページに案内するだけ。URLに移動します。
            try
            {
                // URLを開く
                driver.Navigate().GoToUrl(@"https://marshmallow-qa.com/auth/explanation");
                
            }
            catch (Exception)
            {
                // クソコードなのでエラーは握りつぶす。
                // 手動で頑張って操作して。
            }

        }


        private void Get_marshmallow_button(object sender, RoutedEventArgs e)
        {
            string text = NumMaxText.Text;
            int nummax = int.Parse(text);
            driver.Navigate().GoToUrl(@"https://marshmallow-qa.com/messages");

            ReadOnlyCollection<IWebElement> elements = driver.FindElements(By.ClassName("text-dark"));
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            //string scripts = "var element = driver.getElementById('test'); element.scrollIntoView(); element.click();";
            while (true)
            {

                // 欲しい要素を取得する。マシュマロのコメントは現状text-darkになっている。
                elements = driver.FindElements(By.ClassName("text-dark"));


                System.Threading.Thread.Sleep(2000);

                if (elements.Count < nummax)
                {


                    
                    try
                    {
                        // クリックを押すとボタンが見える位置に移動するのでクリックする
                        // なぜか初回は失敗するので2回押す
                        driver.FindElement(By.CssSelector(".btn.btn-secondary")).Click();
                    }
                    catch
                    {
                        //移動のためのクリックなのでエラーは握りDescription つぶす。
                    }
                    // ボタンの描画まで時間がかかる場合があるので待つ。
                    System.Threading.Thread.Sleep(3000);

                    try
                    {
                        // 2回目のクリック。こっちはちゃんとクリックできる。
                        driver.FindElement(By.CssSelector(".btn.btn-secondary")).Click();
                    }
                    catch (NoSuchElementException)
                    {
                        // この段階で要素が見つからないときは全て表示し終わってボタンがなくなったとき。
                        Console.WriteLine("もっと見るボタンが押せなくなりました。");
                        break;
                    }
                }
                else
                {
                    break;
                }
                

            }

            elements = driver.FindElements(By.ClassName("text-dark"));
            string date = "marshmallow_" + DateTime.Now.ToString("MM月dd日HH時mm分ss秒") +".txt";
            Console.WriteLine("出力先:" + date);
            StreamWriter sw = File.CreateText(date);
            int count = 0;
            foreach (var x in elements)
            {
                count++;
                sw.WriteLine(count);
                sw.WriteLine(x.Text);
                sw.WriteLine("");
                if (count >= nummax)
                {
                    break;
                }
            }

            sw.Close();
            MessageBox.Show( (count) +"件マシュマロを取得しました。");

        }

        private void ev_set(object sender, RoutedEventArgs e)
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);

            driver.Navigate().GoToUrl(@"https://marshmallow-qa.com/messages");
            Console.WriteLine(driver.FindElement(By.ClassName("text-dark")).Text);

        }


        //

        protected virtual void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            driver.Quit();
        }
    }
}
