using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http.Json;
using System.Collections;
using System.Runtime.InteropServices.ComTypes;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace postalCodeApp
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public ObservableCollection<personalAddress> postalcodeRows { get; set; }
        private int testCounter = 0;

        public MainWindow()
        {
            InitializeComponent();

            postalcodeRows = new ObservableCollection<personalAddress> { };
            postalCodeGrid.ItemsSource = postalcodeRows;

        }

        /// <summary>
        /// 郵便番号APIから住所の情報を取得
        /// </summary>
        async private void postalCodeApiGet(string postalCode)
        {

            // 郵便番号API連携
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage responseMessage = await client.GetAsync($"https://zipcloud.ibsnet.co.jp/api/search?zipcode={postalCode}");
                var readString = await responseMessage.Content.ReadAsStringAsync();
                var json = JsonSerializer.Deserialize<Rootobject>(readString);

                setData(json.results.First());

            }
        }

        /// <summary>
        /// 取得したデータから情報をDataGridへ登録/表示する
        /// </summary>
        /// <param name="personal"></param>
        private void setData(personalAddress personal)
        {
            postalcodeRows.Add(new personalAddress 
            { 
                address1 = personal.address1,
                address2 = personal.address2,
                address3 = personal.address3,
                kana1 = personal.kana1,
                kana2 = personal.kana2,
                kana3 = personal.kana3,
                prefcode = personal.prefcode,
                zipcode = personal.zipcode
            });
        }

        /// <summary>
        /// 検索ボタンがクリックされたら入力された郵便番号から住所を検索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void searchbtn_Click(object sender, RoutedEventArgs e)
        {

            string ErrorMsg = string.Empty;

            try
            {
                if (isvalidatePostalCode())
                {
                    postalCodeApiGet(this.postalCode.Text);
                }
                else
                {
                    throw new Exception(message: "郵便番号が正しくありません。再度入力をお願いします。");
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"エラー",MessageBoxButton.OK,MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// 正常な郵便番号かチェックし結果を返却
        /// </summary>
        /// <returns></returns>
        private bool isvalidatePostalCode()
        {
            string pattern = @"^\d{3}-?\d{4}$";
            return Regex.IsMatch(this.postalCode.Text, pattern);
        }

    }

    public class Rootobject
    {
        public object message { get; set; }
        public personalAddress[] results { get; set; }
        public int status { get; set; }
    }

    public class personalAddress
    {
        public string? address1 { get; set; }
        public string? address2 { get; set; }
        public string? address3 { get; set; }
        public string? kana1 { get; set; }
        public string? kana2 { get; set; }
        public string? kana3 { get; set; }
        public string? prefcode { get; set; }
        public string? zipcode { get; set; }
    }
}
