
HttpClient client=new HttpClient();
Progress<HttpDownloadProgress> p =new(o => {

    Console.WriteLine(o.BytesReceived.ToString());
});
client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.61 Safari/537.36");

 await client.GetByteArrayAsync(new Uri("https://vipspeedtest8.wuhan.net.cn:8080/download"),p, CancellationToken.None);
 
Console.Read();
 