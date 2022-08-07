using ProductLabLifeHack;
using Newtonsoft.Json.Linq;
using ClosedXML.Excel;

string keyFilePath = @"C:\Users\ryzhuk\Desktop\tmp\ProductLab\ProductLab\Keys.txt";
List<string> keys = new();

try
{
    if (!File.Exists(keyFilePath))
        throw new FileNotFoundException(keyFilePath);

    keys = File.ReadAllLines(keyFilePath).ToList();

    using IXLWorkbook workbook = new XLWorkbook();

    foreach (string line in keys)
    {
        var productList = GetProductsByKeyAsync(line).Result;

        Export<Product>(productList, line, workbook);
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

//Get JSON products without advertisements
async Task<IList<Product>?> GetProductsByKeyAsync(string key)
{
    try
    {
        string searchPath = $"https://search.wb.ru/exactmatch/ru/common/v4/search?appType=1&couponsGeo=12,3,18,15,21&curr=rub&dest=-1029256,-102269,-2162196,-1257786&emp=0&lang=ru&locale=ru&pricemarginCoeff=1.0&query={key}&reg=0&regions=68,64,83,4,38,80,33,70,82,86,75,30,69,22,66,31,40,1,48,71&resultset=catalog&sort=popular&spp=0&suppressSpellcheck=false";
        string jsonProducts = string.Empty;

        if (key == null)
            throw new ArgumentNullException();

        HttpClient client = new HttpClient();
        HttpResponseMessage response = await client.GetAsync(searchPath);

        if (response.IsSuccessStatusCode)
            jsonProducts = response.Content.ReadAsStringAsync().Result;

        var products = JObject.Parse(jsonProducts)["data"]["products"]
            .ToObject<List<Product>>();

        foreach (var item in products)
            item.Price /= 100;

        return products;
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        return null;
    }
}

//Export to excel via ClosedXml
void Export<T>(IList<T> list, string sheetName, IXLWorkbook workbook)
{
    try
    {
        if (list == null || sheetName == null || workbook == null)
            throw new ArgumentNullException();

        workbook.AddWorksheet(sheetName).FirstCell().InsertTable<T>(list, false);
        workbook.SaveAs("asd.xlsx");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}