using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Caching;
using System.Xml;

namespace EfasisCMS
{
    public class n11
    {
        clstblpazaryerleri pazaryerisinifi = new clstblpazaryerleri();
        public void test1()
        {
            DataTable dtpazaryeri = pazaryerisinifi.SatirGetirisimile("N11");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            System.Net.ServicePointManager.Expect100Continue = false;
            n11CategoryService.Authentication authentication = new n11CategoryService.Authentication();
            authentication.appKey = dtpazaryeri.Rows[0]["appKey"].ToString();
            authentication.appSecret = dtpazaryeri.Rows[0]["appSecret"].ToString();

            n11CategoryService.GetTopLevelCategoriesRequest getTopLevelCategoriesRequest = new n11CategoryService.GetTopLevelCategoriesRequest();
            getTopLevelCategoriesRequest.auth = authentication;

            n11CategoryService.GetTopLevelCategoriesResponse getTopLevelCategoriesResponse = new n11CategoryService.GetTopLevelCategoriesResponse();

            n11CategoryService.CategoryServicePortClient categoryServicePortClient = new n11CategoryService.CategoryServicePortClient();
            getTopLevelCategoriesResponse = categoryServicePortClient.GetTopLevelCategories(getTopLevelCategoriesRequest);
            

            
            //com.n11.api.Authentication authentication = new com.n11.api.Authentication();
            // authentication.appKey = "3a3ae3a5-cd33-40ee-85d9-c3adca501544";
            // authentication.appSecret = "gJZrLmueysq9b4W8";

            //com.n11.api.PagingData pagingData = new com.n11.api.PagingData();
            //pagingData.currentPage = 0;
            //pagingData.pageSize = 100;

            //com.n11.api.GetCategoryAttributesRequest getCategoryAttributesRequest = new com.n11.api.GetCategoryAttributesRequest();
            //getCategoryAttributesRequest.auth = authentication;
            //getCategoryAttributesRequest.categoryId = 1002306;

            //com.n11.api.CategoryServicePortService categoryServicePortService = new com.n11.api.CategoryServicePortService();
            //categoryServicePortService.getcategory
            //com.n11.api.GetCategoryAttributesResponse getCategoryAttributesResponse = categoryServicePortService.GetCategoryAttributes(getCategoryAttributesRequest);
            ////CategoryServicePort port = new CategoryServicePortService().getCategoryServicePortSoap11();
            ////com.n11.api.CategoryServicePortService srr = new com.n11.api.CategoryServicePortService();
            ////srr.
        }

        public void SehirleriGetir()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            System.Net.ServicePointManager.Expect100Continue = false;
            n11CityService.GetCitiesRequest getCitiesRequest = new n11CityService.GetCitiesRequest();

            n11CityService.GetCitiesResponse getCitiesResponse = new n11CityService.GetCitiesResponse();
            n11CityService.CityServicePortClient cityServicePortClient = new n11CityService.CityServicePortClient();
            var data = cityServicePortClient.GetCities(getCitiesRequest);
        }
        public List<n11KategoriOzellikleri> KategoriOzellikleriniGetir(int KategoriID)
        {
            
            if (HttpContext.Current.Cache[KategoriID.ToString()] != null)
            {
                List<n11KategoriOzellikleri> liste = (List<n11KategoriOzellikleri>)HttpContext.Current.Cache[KategoriID.ToString()];
                return liste;
            }
            else
            {
                DataTable dtpazaryeri = pazaryerisinifi.SatirGetirisimile("N11");

                List<n11KategoriOzellikleri> LstOzellikler = new List<n11KategoriOzellikleri>();


                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                string xmlpostData = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:sch=\"http://www.n11.com/ws/schemas\"><soapenv:Header/><soapenv:Body><sch:GetCategoryAttributesRequest><auth><appKey>" + dtpazaryeri.Rows[0]["appKey"].ToString() + "</appKey><appSecret>" + dtpazaryeri.Rows[0]["appSecret"].ToString() + "</appSecret></auth><categoryId>" + KategoriID + "</categoryId></sch:GetCategoryAttributesRequest></soapenv:Body></soapenv:Envelope>";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.n11.com/ws/CategoryService.wsdl");
                byte[] bytes;
                bytes = System.Text.Encoding.ASCII.GetBytes(xmlpostData);
                request.ContentType = "text/xml; encoding='utf-8'";
                request.ContentLength = bytes.Length;
                request.Method = "POST";
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
                HttpWebResponse response;
                response = (HttpWebResponse)request.GetResponse();


                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream responseStream = response.GetResponseStream();
                    string responseStr = new StreamReader(responseStream).ReadToEnd();
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(responseStr);
                    if (doc.ChildNodes[0].ChildNodes[1].ChildNodes[0].ChildNodes.Count > 1)
                    {
                        XmlNode xn = doc.ChildNodes[0].ChildNodes[1].ChildNodes[0].ChildNodes[1].ChildNodes[1];

                        for (int i = 0; i < xn.ChildNodes.Count; i++)
                        {
                            if (xn.ChildNodes[i]["name"] != null)
                            {
                                n11KategoriOzellikleri oge = new n11KategoriOzellikleri();
                                oge.id = xn.ChildNodes[i]["id"].InnerText;
                                oge.multipleSelect = xn.ChildNodes[i]["multipleSelect"].InnerText;
                                oge.name = xn.ChildNodes[i]["name"].InnerText;
                                oge.priority = xn.ChildNodes[i]["priority"].InnerText;
                                oge.mandatory = xn.ChildNodes[i]["mandatory"].InnerText;

                                if (oge.mandatory == "true")
                                {
                                    List<n11KategoriOzellikValue> LstOzelliklervalue = new List<n11KategoriOzellikValue>();
                                    for (int x = 0; x < xn.ChildNodes[i]["valueList"].ChildNodes.Count; x++)
                                    {
                                        n11KategoriOzellikValue o = new n11KategoriOzellikValue();
                                        o.id = xn.ChildNodes[i]["valueList"].ChildNodes[x]["id"].InnerText;
                                        o.name = xn.ChildNodes[i]["valueList"].ChildNodes[x]["name"].InnerText;
                                        LstOzelliklervalue.Add(o);
                                    }
                                    oge.valueList = LstOzelliklervalue;
                                    LstOzellikler.Add(oge);
                                }
                            }
                        }
                    }
                }
                HttpContext.Current.Cache.Insert(KategoriID.ToString(), LstOzellikler, null, DateTime.Now.AddDays(1), System.Web.Caching.Cache.NoSlidingExpiration);  
                return LstOzellikler;

            }


        }


        public void KategoriOzellikDegerleriniGetir(long kategoriozellikid)
        {

            DataTable dtpazaryeri = pazaryerisinifi.SatirGetirisimile("N11");


            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            System.Net.ServicePointManager.Expect100Continue = false;
            n11CategoryService.Authentication authentication = new n11CategoryService.Authentication();
            authentication.appKey = dtpazaryeri.Rows[0]["appKey"].ToString();
            authentication.appSecret = dtpazaryeri.Rows[0]["appSecret"].ToString();
            n11CategoryService.RequestPagingData pagingData = new n11CategoryService.RequestPagingData();
            pagingData.currentPage = 0;
            pagingData.pageSize = 100;
 
            n11CategoryService.GetCategoryAttributeValueRequest getCategoryAttributeValueRequest = new n11CategoryService.GetCategoryAttributeValueRequest();
            getCategoryAttributeValueRequest.auth = authentication;
            getCategoryAttributeValueRequest.categoryProductAttributeId = kategoriozellikid;
            getCategoryAttributeValueRequest.pagingData = pagingData;

            n11CategoryService.CategoryServicePortClient categoryServicePortClient = new n11CategoryService.CategoryServicePortClient();
            n11CategoryService.GetCategoryAttributeValueResponse getCategoryAttributeValueResponse = new n11CategoryService.GetCategoryAttributeValueResponse();

            getCategoryAttributeValueResponse = categoryServicePortClient.GetCategoryAttributeValue(getCategoryAttributeValueRequest);
            string ccc = "";
            //var secilikategori = from kategori in TyKategoriler
            //                     where kategori.id == selectedvalue
            //                     select kategori;
           


        }

        public List<n11CategoryService.SubCategory> AltKategorileriGetir(long KategoriID)
        {
            //1002234 Müzik
            DataTable dtpazaryeri = pazaryerisinifi.SatirGetirisimile("N11");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            System.Net.ServicePointManager.Expect100Continue = false;
            n11CategoryService.Authentication authentication = new n11CategoryService.Authentication();
            authentication.appKey = dtpazaryeri.Rows[0]["appKey"].ToString();
            authentication.appSecret = dtpazaryeri.Rows[0]["appSecret"].ToString();

            n11CategoryService.GetSubCategoriesRequest getCategoryAttributesRequest = new n11CategoryService.GetSubCategoriesRequest();
            getCategoryAttributesRequest.auth = authentication;
            getCategoryAttributesRequest.categoryId = KategoriID;

            n11CategoryService.GetSubCategoriesResponse getCategoryAttributesResponse = new n11CategoryService.GetSubCategoriesResponse();
            n11CategoryService.CategoryServicePortClient categoryServicePortClient = new n11CategoryService.CategoryServicePortClient();
            getCategoryAttributesResponse = categoryServicePortClient.GetSubCategories(getCategoryAttributesRequest);
            List<n11CategoryService.SubCategory> AltKategoriListesi = new List<n11CategoryService.SubCategory>();
            if (getCategoryAttributesResponse.category[0].subCategoryList != null)
            {
                AltKategoriListesi = getCategoryAttributesResponse.category[0].subCategoryList.ToList();
            }
            return AltKategoriListesi;
            //AltKategoriListesi =
        }
        public List<n11CategoryService.SubCategory> KategorileriGetir()
        {
            DataTable dtpazaryeri = pazaryerisinifi.SatirGetirisimile("N11");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            System.Net.ServicePointManager.Expect100Continue = false;
            n11CategoryService.Authentication authentication = new n11CategoryService.Authentication();
            authentication.appKey = dtpazaryeri.Rows[0]["appKey"].ToString();
            authentication.appSecret = dtpazaryeri.Rows[0]["appSecret"].ToString();
            n11CategoryService.GetTopLevelCategoriesRequest getTopLevelCategoriesRequest = new n11CategoryService.GetTopLevelCategoriesRequest();
            getTopLevelCategoriesRequest.auth = authentication;
            n11CategoryService.GetTopLevelCategoriesResponse getTopLevelCategoriesResponse = new n11CategoryService.GetTopLevelCategoriesResponse();
            n11CategoryService.CategoryServicePortClient categoryServicePortClient = new n11CategoryService.CategoryServicePortClient();
            getTopLevelCategoriesResponse = categoryServicePortClient.GetTopLevelCategories(getTopLevelCategoriesRequest);
            List<n11CategoryService.SubCategory> KategoriListesi = new List<n11CategoryService.SubCategory>();
            KategoriListesi = getTopLevelCategoriesResponse.categoryList.ToList();
            return KategoriListesi;
        }
        public void UrunEkle()
        {
            DataTable dtpazaryeri = pazaryerisinifi.SatirGetirisimile("N11");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            System.Net.ServicePointManager.Expect100Continue = false;
            n11ProductService.Authentication authentication = new n11ProductService.Authentication();
            authentication.appKey = dtpazaryeri.Rows[0]["appKey"].ToString();
            authentication.appSecret = dtpazaryeri.Rows[0]["appSecret"].ToString();
            string strUrl = "https://www.google.com/logos/doodles/2016/bahrain-national-day-2016-6221988579246080-hp2x.jpg";
            string strSellerStockCode = "MaviKod-APIDenemex22432100000000";
           //string strSellerStockCode1 = "KırmızıKod-APIDenemex224321000000000";
            string strAttributeName = "Marka";
            string strAttributeValue = "Diğer";
            string strSkuAttributeKey = "Renk";
            string strSkuAttributeValue = "Mavi";
            //string strSkuAttributeValue1 = "Kırmızı";
            string strProductTitle = "Lorem ipsum";
            string strProductSubtitle = "Lorem ipsum dolor sit amet";
            string strProductSellerCode = "APIDeneme432101000000000";
            string strProductCondition = "1";
            string strShipmentTemplate = "babil";
            string strProductDescription = "Hello World!";
            string strGtin = "0190198066473";
            //string strGtin1 = "0190198066474";
            int setOrderValue = 1;
            int quantityValue = 10;
            //int quantityValue1 = 20;
            int categoryIdValue = 1001158;
            int priceValue = 50;
            int currencyTypeValue = 1;
            int approvalStatusValue = 1;
            int preparingDayValue = 3;


            //Product Images
            List<n11ProductService.ProductImage> productImageList = new List<n11ProductService.ProductImage>();
            n11ProductService.ProductImage productImage = new n11ProductService.ProductImage();
            productImage.url = strUrl;
            productImage.order = setOrderValue.ToString();
            productImageList.Add(productImage);

            //Sku Attributes
            List<n11ProductService.ProductAttributeRequest> skuAttributeRequestList = new List<n11ProductService.ProductAttributeRequest>();
            n11ProductService.ProductAttributeRequest skuAttributeRequest = new n11ProductService.ProductAttributeRequest();
            skuAttributeRequest.name = strSkuAttributeKey;
            skuAttributeRequest.value = strSkuAttributeValue;
            skuAttributeRequestList.Add(skuAttributeRequest);


            //List<n11ProductService.ProductAttributeRequest> skuAttributeRequestList1 = new List<n11ProductService.ProductAttributeRequest>();
            //n11ProductService.ProductAttributeRequest skuAttributeRequest1 = new n11ProductService.ProductAttributeRequest();
            //skuAttributeRequest1.name = strSkuAttributeKey;
            //skuAttributeRequest1.value = strSkuAttributeValue1;
            //skuAttributeRequestList1.Add(skuAttributeRequest1);



            //Product Sku
            List<n11ProductService.ProductSkuRequest> stockItems = new List<n11ProductService.ProductSkuRequest>();


            n11ProductService.ProductSkuRequest sku = new n11ProductService.ProductSkuRequest();
            sku.sellerStockCode =strSellerStockCode;
            sku.attributes = skuAttributeRequestList.ToArray();
            sku.quantity = quantityValue.ToString();
            sku.gtin = strGtin;
            sku.oem = "";
            sku.optionPrice = priceValue;

            //n11ProductService.ProductSkuRequest sku1 = new n11ProductService.ProductSkuRequest();
            //sku1.sellerStockCode = strSellerStockCode1;
            //sku1.attributes = skuAttributeRequestList1.ToArray();
            //sku1.quantity = quantityValue1.ToString();
            //sku1.gtin = strGtin1;
            //sku1.optionPrice = priceValue;
            //sku1.oem = "";
            stockItems.Add(sku);
            //stockItems.Add(sku1);

            //Category Request
            n11ProductService.CategoryRequest categoryRequest = new n11ProductService.CategoryRequest();
            categoryRequest.id = categoryIdValue;

            //Product Attribute
            n11ProductService.ProductAttributeRequest productAttribute = new n11ProductService.ProductAttributeRequest();
             productAttribute.name = strAttributeName;
             productAttribute.value = strAttributeValue;



            List<n11ProductService.ProductAttributeRequest> productAttributeRequestList = new List<n11ProductService.ProductAttributeRequest>();
            productAttributeRequestList.Add(productAttribute);


            n11ProductService.ProductRequest productRequest = new n11ProductService.ProductRequest();
            productRequest.title = strProductTitle;
            productRequest.subtitle = strProductSubtitle;
            productRequest.description = strProductDescription;
            productRequest.category = categoryRequest;
            productRequest.productSellerCode = strProductSellerCode;
            productRequest.price = priceValue;
            productRequest.currencyType = currencyTypeValue.ToString();
            productRequest.images = productImageList.ToArray();
            productRequest.approvalStatus = approvalStatusValue.ToString();
            productRequest.preparingDay = preparingDayValue.ToString();
            productRequest.stockItems = stockItems.ToArray();
            productRequest.productCondition = strProductCondition;
            productRequest.shipmentTemplate = strShipmentTemplate;
            productRequest.attributes = productAttributeRequestList.ToArray();
            productRequest.domestic = true;
            n11ProductService.SaveProductRequest saveProductRequest = new n11ProductService.SaveProductRequest();
            saveProductRequest.auth = authentication;
            saveProductRequest.product = productRequest;
            n11ProductService.ProductServicePortClient productServicePortClient = new  n11ProductService.ProductServicePortClient();
            n11ProductService.SaveProductResponse saveProductResponse = productServicePortClient.SaveProduct(saveProductRequest);
         
        }

        public string UrunEkleParametrik(string UrunBaslik,string UrunAltbaslik,string StokKodu,string saticikodu, string resimurl, string teslimatsablonu, string urunaciklama,int stokmiktari,int kategoriid,decimal fiyat,int hazirlikgunu, 
            List<n11ProductService.ProductAttributeRequest> skuAttributeRequestList)
        {
            DataTable dtpazaryeri = pazaryerisinifi.SatirGetirisimile("N11");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                System.Net.ServicePointManager.Expect100Continue = false;
                n11ProductService.Authentication authentication = new n11ProductService.Authentication();
            authentication.appKey = dtpazaryeri.Rows[0]["appKey"].ToString();
            authentication.appSecret = dtpazaryeri.Rows[0]["appSecret"].ToString();

            string strUrl = resimurl;
                string strSellerStockCode = StokKodu;
                // string strSellerStockCode1 = "KırmızıKod-APIDenemex224321000000000";
                string strAttributeName = "Marka";
                string strAttributeValue = "Diğer";


            string strProductTitle = UrunBaslik;
            string strProductSubtitle = UrunAltbaslik;
            string strProductSellerCode = saticikodu;
                string strProductCondition = "1";
            string strShipmentTemplate = teslimatsablonu;
            string strProductDescription = urunaciklama;
                string strGtin = "0190198066473";
                //string strGtin1 = "0190198066474";
                int setOrderValue = 1;
            int quantityValue = stokmiktari;
                //int quantityValue1 = 20;
                int categoryIdValue = kategoriid;
                decimal priceValue = fiyat;
                int currencyTypeValue = 1;
                int approvalStatusValue = 1;
                int preparingDayValue = hazirlikgunu;


                //Product Images
                List<n11ProductService.ProductImage> productImageList = new List<n11ProductService.ProductImage>();
                n11ProductService.ProductImage productImage = new n11ProductService.ProductImage();
                productImage.url = strUrl;
                productImage.order = setOrderValue.ToString();
                productImageList.Add(productImage);

                //Sku Attributes
                //List<n11ProductService.ProductAttributeRequest> skuAttributeRequestList = new List<n11ProductService.ProductAttributeRequest>();
                //n11ProductService.ProductAttributeRequest skuAttributeRequest = new n11ProductService.ProductAttributeRequest();
                //skuAttributeRequest.name = strSkuAttributeKey;
                //skuAttributeRequest.value = strSkuAttributeValue;
                //skuAttributeRequestList.Add(skuAttributeRequest);
 

                //Product Sku
                List<n11ProductService.ProductSkuRequest> stockItems = new List<n11ProductService.ProductSkuRequest>();


                n11ProductService.ProductSkuRequest sku = new n11ProductService.ProductSkuRequest();
                sku.sellerStockCode = strSellerStockCode;
                sku.attributes = skuAttributeRequestList.ToArray();
                sku.quantity = quantityValue.ToString();
                sku.gtin = strGtin;
                sku.oem = "";
                sku.optionPrice = priceValue;

                
                stockItems.Add(sku);
                

                //Category Request
                n11ProductService.CategoryRequest categoryRequest = new n11ProductService.CategoryRequest();
                categoryRequest.id = categoryIdValue;

                //Product Attribute
                n11ProductService.ProductAttributeRequest productAttribute = new n11ProductService.ProductAttributeRequest();
                productAttribute.name = strAttributeName;
                productAttribute.value = strAttributeValue;
                List<n11ProductService.ProductAttributeRequest> productAttributeRequestList = new List<n11ProductService.ProductAttributeRequest>();
                productAttributeRequestList.Add(productAttribute);


                n11ProductService.ProductRequest productRequest = new n11ProductService.ProductRequest();
                productRequest.title = strProductTitle;
                productRequest.subtitle = strProductSubtitle;
                productRequest.description = "dasdasdasfsdgdgd";
                productRequest.category = categoryRequest;
                productRequest.productSellerCode = strProductSellerCode;
                productRequest.price = priceValue;
                productRequest.currencyType = currencyTypeValue.ToString();
                productRequest.images = productImageList.ToArray();
                productRequest.approvalStatus = approvalStatusValue.ToString();
                productRequest.preparingDay = preparingDayValue.ToString();
                productRequest.stockItems = stockItems.ToArray();
                productRequest.productCondition = strProductCondition;
                productRequest.shipmentTemplate = strShipmentTemplate;
                productRequest.attributes = skuAttributeRequestList.ToArray();
                productRequest.domestic = true;

                n11ProductService.SaveProductRequest saveProductRequest = new n11ProductService.SaveProductRequest();
                saveProductRequest.auth = authentication;
                saveProductRequest.product = productRequest;

                n11ProductService.ProductServicePortClient productServicePortClient = new n11ProductService.ProductServicePortClient();
                n11ProductService.SaveProductResponse saveProductResponse = productServicePortClient.SaveProduct(saveProductRequest);

            string cevap = "";
            if (saveProductResponse.result.errorMessage != null)
            {

                cevap = "N11 Hata: " + saveProductResponse.result.errorMessage;
            }
            return cevap;

        }


        public void UrunleriGetir()
        {
            DataTable dtpazaryeri = pazaryerisinifi.SatirGetirisimile("N11");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            System.Net.ServicePointManager.Expect100Continue = false;
            n11ProductService.Authentication authentication = new n11ProductService.Authentication();
            authentication.appKey = dtpazaryeri.Rows[0]["appKey"].ToString();
            authentication.appSecret = dtpazaryeri.Rows[0]["appSecret"].ToString();
            n11ProductService.RequestPagingData requestPagingData = new n11ProductService.RequestPagingData();
            requestPagingData.currentPage = 0;
            requestPagingData.pageSize = 3;

            n11ProductService.GetProductListRequest getProductListRequest = new n11ProductService.GetProductListRequest();
            getProductListRequest.auth = authentication;
            getProductListRequest.pagingData = requestPagingData;

            n11ProductService.ProductServicePortClient productServicePort = new n11ProductService.ProductServicePortClient();
            n11ProductService.GetProductListResponse getProductListResponse = new n11ProductService.GetProductListResponse();
            getProductListResponse = productServicePort.GetProductList(getProductListRequest);
        }

        public List<n11ShipmentService.ShipmentApiModel> TeslimatSablonlariGetir()
        {
            DataTable dtpazaryeri = pazaryerisinifi.SatirGetirisimile("N11");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            System.Net.ServicePointManager.Expect100Continue = false;
            n11ShipmentService.Authentication authentication = new n11ShipmentService.Authentication();
            authentication.appKey = dtpazaryeri.Rows[0]["appKey"].ToString();
            authentication.appSecret = dtpazaryeri.Rows[0]["appSecret"].ToString();


            n11ShipmentService.GetShipmentTemplateListRequest getShipmentTemplateList = new n11ShipmentService.GetShipmentTemplateListRequest();
            getShipmentTemplateList.auth = authentication;


            n11ShipmentService.ShipmentServicePortClient port = new n11ShipmentService.ShipmentServicePortClient();
            n11ShipmentService.GetShipmentTemplateListResponse getShipmentTemplateListResponse = port.GetShipmentTemplateList(getShipmentTemplateList);
            List<n11ShipmentService.ShipmentApiModel> liste = getShipmentTemplateListResponse.shipmentTemplates.ToList();
            return liste;
        }


    }
}

public class n11KategoriOzellikleri
{
    public string id { get; set; }
    public string mandatory { get; set; }
    public string multipleSelect { get; set; }
    public string name { get; set; }
    public string priority { get; set; }
    public List<n11KategoriOzellikValue> valueList { get; set; }
}
public class n11KategoriOzellikValue
{
    public string id { get; set; }
    public string name { get; set; }
}