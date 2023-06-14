using AngleSharp;
using AngleSharp.Dom;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace SamishopV2_Template_1.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration configurationSetting;
        public HomeController(IConfiguration configuration)
        {
            configurationSetting = configuration;
        }

       [Route("/{urlName?}/{urlName2?}")]
        public async Task<ActionResult> Index(string urlName, string urlName2)
        {
            string hexPaletaColores = "{" +
                "green: {color1: '#E3E8C9',color2: '#CED69A',color3: '#A9B546',color4: '#777C56'}," +
                "blue: {color1: '#C9D4E8',color2: '#CED69A',color3: '#4672B5',color4: '#56617C'}," +
                "purple: {color1: '#D7C9E8',color2: '#B49AD6',color3: '#7746B5',color4: '#62567C'}," +
                "red: {color1: '#E8C9C9',color2: '#D69A9A',color3: '#B54646',color4: '#7C5656'}," +
                "orange: {color1: '#FFD9BE',color2: '#E5AC78',color3: '#D47540',color4: '#7C6156'}," +
                "dark: {color1: '#EBEBEB',color2: '#C8C8C8',color3: '#959595',color4: '#585858'}" +
            "}";
            var htmlFather = "";
            string htmlChildren = null;
            var UrlGoogleStorage = configurationSetting["UrlCdnStorage"];
            var UrlDefaultTemplate = configurationSetting["UrlDefaultTemplate"];
            var UrlApiCatalog = configurationSetting["UrlApiCatalog"];
            string VariableIsActive = configurationSetting["VariableIsActive"];
            string SaleformJavascript = configurationSetting["SaleformJavascript"];
            string UrlPlantilla = configurationSetting["UrlPlantilla"];

            string hostFolderClient = "";
            string urlNameFinal = null;
            var UrlCdnClient = "";
            var UrlCdnClientPlantilla = UrlPlantilla;
            var UrlCdnClientPrincipal = configurationSetting["UrlCdnStoragePrincipal"];
            string urlName3 = "";

            var valueRandom = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
            string myuri = Request.Headers.Host;
            string[] separatedUrl = myuri.Split('/');
            try
            {
                string hostFolderClientHost = Request.Headers.Host;
                hostFolderClientHost = hostFolderClientHost.ToLower();
                hostFolderClientHost = hostFolderClientHost.Replace("/", "");
                hostFolderClientHost = hostFolderClientHost.Replace("www.", "");

                urlName3 = hostFolderClientHost;
                //urlName3 = "qafrankcatering.s1a2m3i4.com";
                hostFolderClient = urlName3;

                UrlCdnClient = UrlGoogleStorage + "/" + urlName3;

                // INICIO CONSUME TEMPLATE.HTML DE PLANTILLA TIENDA
                UrlCdnClientPlantilla = UrlPlantilla;
                // FIN CONSUME TEMPLATE.HTML DE PLANTILLA TIENDA

                var config = Configuration.Default.WithDefaultLoader();
                var context = BrowsingContext.New(config);
                
                var UrlGoogleTemplate = UrlCdnClientPlantilla + UrlDefaultTemplate + "/template" + ".html" + "?v=" + valueRandom;
                var documentFather = await context.OpenAsync(UrlGoogleTemplate);
                if (documentFather.StatusCode != HttpStatusCode.OK) throw new Exception();
                htmlFather = documentFather.Source.Text;
                
                var UrlGoogleHeader = UrlCdnClientPlantilla + UrlDefaultTemplate + "/header/" + "header" + ".html" + "?v=" + valueRandom;
                var documentHeader = await context.OpenAsync(UrlGoogleHeader);
                if (documentHeader.StatusCode != HttpStatusCode.OK) throw new Exception();
                string htmlDocumentHeader = documentHeader.Source.Text;
                htmlFather = htmlFather.Replace("[[HTML_HEADER_V1]]", htmlDocumentHeader);
                
                var UrlGoogleFooter = UrlCdnClientPlantilla + UrlDefaultTemplate + "/footer/" + "footer" + ".html" + "?v=" + valueRandom;
                var documentFooter = await context.OpenAsync(UrlGoogleFooter);

                if (documentFooter.StatusCode != HttpStatusCode.OK) throw new Exception();
                htmlFather = htmlFather.Replace("[[HTML_FOOTER_V1]]", documentFooter.Source.Text);

                if (urlName == null)
                {
                    urlName = "home";
                    urlNameFinal = urlName;
                }
                else
                {
                    urlName = urlName.ToLower();

                    if (urlName2 != null)
                    {
                        urlName2 = urlName2.ToLower();
                        string extension = Path.GetExtension(urlName2);
                        if (extension == ".html")
                        {
                            urlName2 = urlName2.Substring(0, urlName2.Length - extension.Length);
                        }
                    }
                    else
                    {
                        string extension = Path.GetExtension(urlName);
                        if (extension == ".html")
                        {
                            urlName = urlName.Substring(0, urlName.Length - extension.Length);
                        }
                    }
                    urlNameFinal = urlName;
                }

                if (urlName != null && urlName2 != null)
                {
                    urlName2 = urlName2.ToLower();
                    urlNameFinal = urlName + "_" + urlName2.ToLower();
                }

                string codeProduct = "";
                if (urlName != null) {
                    var urlNameAplit = urlName.Split(",");
                    if (urlNameAplit.Count() > 1){
                        codeProduct = urlNameAplit[1].Trim();
                    }
                    else
                    {
                        urlName = urlName.ToLower(); 
                    }
                }

                if (codeProduct == "")
                {
                    var TypePage = "paginas_contenido";
                    if (urlName == null)
                    {
                        urlName = "home";
                    }
                    else
                    {
                        urlName = urlName.ToLower();
                    }
                    if (urlName != null && urlName2 != null)
                    {
                        if (!urlName.Equals("blog"))
                        {
                            TypePage = "paginas_aplicacion";
                        }
                    }

                    if (urlName == "f")
                    {

                        TypePage = "paginas_aplicacion";
                        var DocumentCatalogo = await context.OpenAsync(UrlCdnClientPlantilla + UrlDefaultTemplate + "/" + TypePage + "/" + "process_catalogo" + ".html" + "?v=" + valueRandom);
                        if (DocumentCatalogo.StatusCode == HttpStatusCode.OK)
                        {
                            htmlChildren = DocumentCatalogo.Source.Text;
                        }
                        else
                        {
                            htmlChildren = null;
                        }
                    }
                    else
                    {
                        
                        var documentChildren = await context.OpenAsync(UrlCdnClientPlantilla + UrlDefaultTemplate + "/" + TypePage + "/" + urlNameFinal + ".html" + "?v=" + valueRandom);

                        if (documentChildren.StatusCode != HttpStatusCode.OK)
                        {    
                            var DocumentCatalogo = await context.OpenAsync(UrlCdnClientPlantilla + UrlDefaultTemplate + "/" + TypePage + "/" + urlNameFinal + ".html" + "?v=" + valueRandom);
                            if (DocumentCatalogo.StatusCode == HttpStatusCode.OK)
                            {
                                htmlChildren = DocumentCatalogo.Source.Text;
                            }
                            else
                            {
                                htmlChildren = null;
                            }
                        }
                        else
                        {
                            htmlChildren = documentChildren.Source.Text;
                        }
                    }
                }
                else if (codeProduct != "")
                {

                    var DocumentProduct = await context.OpenAsync(UrlCdnClientPlantilla + UrlDefaultTemplate + "/paginas_aplicacion/" + "producto_detalle" + ".html" + "?v=" + valueRandom);
                    htmlChildren = DocumentProduct.Source.Text;
                    bool resultado_servidor = false;
                    dynamic result = null;
                    if (!htmlChildren.Contains(SaleformJavascript))
                    {
                        HttpClient Client = new HttpClient();
                        var Result = await Client.GetAsync(UrlApiCatalog + "/datoscatalogo/" + "saleform/" + hostFolderClient + "/idproducto/" + codeProduct);
                        var Content = Result.Content.ReadAsStringAsync().Result;
                        if (Result.StatusCode == HttpStatusCode.OK)
                        {
                            htmlChildren = DocumentProduct.Source.Text;
                            result = JsonConvert.DeserializeObject(Content);
                            resultado_servidor = true;
                        }
                        else
                        {
                            urlName = "home";
                        }
                    }
                    if (resultado_servidor)
                    {
                        var result_obj_datos_Catalogo = result.obj.datos_Catalogo[0];
                        string datos_Catalogo_sku_padre = result_obj_datos_Catalogo.sku_padre;
                        string datos_Catalogo_item_title = result_obj_datos_Catalogo.item_title;

                        string datos_Catalogo_item_titulo = result_obj_datos_Catalogo.seo_titulo;
                        string datos_Catalogo_palabras_clave = result_obj_datos_Catalogo.seo_palabras_clave;
                        string datos_Catalogo_descripcion = result_obj_datos_Catalogo.seo_descripcion;
                        string datos_Catalogo_tags_promociones = result_obj_datos_Catalogo.tags_promociones;
                        string datos_Catalogo_url_producto = result_obj_datos_Catalogo.url_producto;
                        string datos_Catalogo_descrip_corta = result_obj_datos_Catalogo.descrip_corta;

                        string datos_Catalogo_descripcion1_titulo = result_obj_datos_Catalogo.descripcion1_titulo;
                        string datos_Catalogo_descripcion2_titulo = result_obj_datos_Catalogo.descripcion2_titulo;
                        string datos_Catalogo_descripcion3_titulo = result_obj_datos_Catalogo.descripcion3_titulo;

                        string datos_Catalogo_descripcion1_detalle = result_obj_datos_Catalogo.descripcion1_detalle;
                        string datos_Catalogo_descripcion2_detalle = result_obj_datos_Catalogo.descripcion2_detalle;
                        string datos_Catalogo_descripcion3_detalle = result_obj_datos_Catalogo.descripcion3_detalle;

                        string datos_variaciones_sku = "";
                        decimal datos_variaciones_price = 0;
                        decimal datos_variaciones_sale_price = 0;
                        decimal datos_variaciones_cantidad = 0;
                        string datos_variaciones_atributo1_titulo = "";
                        string datos_variaciones_atributo1_valor = "";
                        string datos_variaciones_atributo2_titulo = "";
                        string datos_variaciones_atributo2_valor = "";
                        string datos_variaciones_atributo3_titulo = "";
                        string datos_variaciones_atributo3_valor = "";

                        string datos_variaciones_url1_imagen_sku = "";
                        string datos_variaciones_url2_imagen_sku = "";
                        string datos_variaciones_url3_imagen_sku = "";
                        string datos_variaciones_url4_imagen_sku = "";
                        string datos_variaciones_url5_imagen_sku = "";
                        string datos_variaciones_url6_imagen_sku = "";

                        List<string> VariationsTitleOne = new List<string>();
                        VariationsTitleOne.Add("");
                        VariationsTitleOne.Add("");
                        VariationsTitleOne.Add("");
                        List<HashSet<string>> variationsList = new List<HashSet<string>>();
                        variationsList.Add(new HashSet<string>());
                        variationsList.Add(new HashSet<string>());
                        variationsList.Add(new HashSet<string>());

                        for (int i = 0; i < result.obj.datos_variaciones.Count; i++)
                        {
                            var result_obj_datos_variaciones = result.obj.datos_variaciones[i];
                            if (i == 0)
                            {
                                datos_variaciones_sku = result_obj_datos_variaciones.sku;
                                datos_variaciones_price = result_obj_datos_variaciones.price;
                                datos_variaciones_sale_price = result_obj_datos_variaciones.sale_price;
                                datos_variaciones_cantidad = result_obj_datos_variaciones.cantidad;
                                datos_variaciones_atributo1_titulo = result_obj_datos_variaciones.atributo1_titulo;
                                datos_variaciones_atributo1_valor = result_obj_datos_variaciones.atributo1_valor;
                                datos_variaciones_atributo2_titulo = result_obj_datos_variaciones.atributo2_titulo;
                                datos_variaciones_atributo2_valor = result_obj_datos_variaciones.atributo2_valor;
                                datos_variaciones_atributo3_titulo = result_obj_datos_variaciones.atributo3_titulo;
                                datos_variaciones_atributo3_valor = result_obj_datos_variaciones.atributo3_valor;
                                datos_variaciones_url1_imagen_sku = result_obj_datos_variaciones.url1_imagen_sku;
                                datos_variaciones_url2_imagen_sku = result_obj_datos_variaciones.url2_imagen_sku;
                                datos_variaciones_url3_imagen_sku = result_obj_datos_variaciones.url3_imagen_sku;
                                datos_variaciones_url4_imagen_sku = result_obj_datos_variaciones.url4_imagen_sku;
                                datos_variaciones_url5_imagen_sku = result_obj_datos_variaciones.url5_imagen_sku;
                                datos_variaciones_url6_imagen_sku = result_obj_datos_variaciones.url6_imagen_sku;
                            }

                            string datos_variaciones_atributo1_titulo_temp = result_obj_datos_variaciones.atributo1_titulo;
                            string datos_variaciones_atributo1_valor_temp = result_obj_datos_variaciones.atributo1_valor;
                            if(datos_variaciones_atributo1_titulo_temp != "" && datos_variaciones_atributo1_valor_temp != "") {
                                VariationsTitleOne[0] = datos_variaciones_atributo1_titulo_temp;
                                variationsList[0].Add(datos_variaciones_atributo1_valor_temp);
                            }

                            string datos_variaciones_atributo2_titulo_temp = result_obj_datos_variaciones.atributo2_titulo;
                            string datos_variaciones_atributo2_valor_temp = result_obj_datos_variaciones.atributo2_valor;
                            if(datos_variaciones_atributo2_titulo_temp != "" && datos_variaciones_atributo2_valor_temp != ""){
                                VariationsTitleOne[1] = datos_variaciones_atributo2_titulo_temp;
                                variationsList[1].Add(datos_variaciones_atributo2_valor_temp);
                            }

                            string datos_variaciones_atributo3_valor_temp = result_obj_datos_variaciones.atributo3_valor;
                            string datos_variaciones_atributo3_titulo_temp = result_obj_datos_variaciones.atributo3_titulo;
                            if(datos_variaciones_atributo3_valor_temp != "" && datos_variaciones_atributo3_titulo_temp != "") {
                                VariationsTitleOne[2] = datos_variaciones_atributo3_titulo_temp;
                                variationsList[2].Add(datos_variaciones_atributo3_valor_temp);
                            }
                        }

                        string ProductVariationSectionHtml = DocumentProduct.QuerySelector("[id='product-variation-section']").InnerHtml.Trim();
                        htmlChildren = htmlChildren.Replace(ProductVariationSectionHtml, "[[PRODUCT_VARIATION_SECTION_HTML]]");

                        string ProductVariationItemHtml = DocumentProduct.QuerySelector("[id='product-variation-items']").InnerHtml;
                        ProductVariationSectionHtml = ProductVariationSectionHtml.Replace(ProductVariationItemHtml, "[[PRODUCT_VARIATION_VALUE_HTML]]");

                        int index = 0;
                        string AllProductVariationSectionHtml = "";
                        foreach (var valueTitle in VariationsTitleOne)
                        {
                            string NewProductVariationSectionHtml = ProductVariationSectionHtml;
                            string AllProductVariationItemHtml = "";

                            if (valueTitle != "")
                            {
                                foreach (var value in variationsList[index])
                                {
                                    string NewProductVariationItemHtml = ProductVariationItemHtml;
                                    NewProductVariationItemHtml = NewProductVariationItemHtml.Replace("[[PRODUCT_VARIATION_VALUE]]", value);
                                    AllProductVariationItemHtml += NewProductVariationItemHtml;
                                }
                                int CountTitle = index + 1;
                                NewProductVariationSectionHtml = NewProductVariationSectionHtml
                                    .Replace("[[PRODUCT_VARIATION_VALUE_HTML]]", AllProductVariationItemHtml)
                                    .Replace("[[PRODUCT_VARIATION_NAME]]", valueTitle)
                                    .Replace("[[PRODUCT_VARIATION_COUNT]]", CountTitle.ToString());
                                AllProductVariationSectionHtml += NewProductVariationSectionHtml;
                            }
                            index++;
                        }


                        htmlChildren = htmlChildren
                            .Replace("[[PRODUCT_VARIATION_SECTION_HTML]]", AllProductVariationSectionHtml)
                            .Replace("[[PRODUCT_TITLE]]", datos_Catalogo_item_title)
                            .Replace("[[PRODUCT_PRICE]]", datos_variaciones_sale_price.ToString())
                            .Replace("[[PRODUCT_SALE_PRICE]]", datos_variaciones_price.ToString())
                            .Replace("[[PRODUCT_DESCRIPTION]]", datos_Catalogo_descrip_corta)
                            .Replace("[[PRODUCT_PRINCIPAL_IMAGE]]", datos_variaciones_url1_imagen_sku)
                            .Replace("[[PRODUCT_PRINCIPAL_SKU]]", datos_variaciones_url1_imagen_sku);
                        htmlFather = htmlFather
                            .Replace("[[OG_PRINCIPAL_IMAGE]]", datos_variaciones_url1_imagen_sku)
                            .Replace("[[OG_TITULO]]", datos_Catalogo_item_titulo)
                            .Replace("[[OG_DESCRIPCION]]", datos_Catalogo_descripcion)
                            .Replace("[[OG_PALABRAS_CLAVE]]", datos_Catalogo_palabras_clave);
                        htmlChildren = htmlChildren
                            .Replace("[[PRODUCT_SKU]]", datos_variaciones_sku)
                            .Replace("[[PRODUCT_DETAILS_TITLE_2]]", datos_Catalogo_descripcion2_titulo)
                            .Replace("[[PRODUCT_DETAILS_TITLE_1]]", datos_Catalogo_descripcion1_titulo)
                            .Replace("[[PRODUCT_DETAILS_TITLE_3]]", datos_Catalogo_descripcion3_titulo)
                            .Replace("[[PRODUCT_DETAILS_TEXT_1]]", datos_Catalogo_descripcion1_detalle)
                            .Replace("[[PRODUCT_DETAILS_TEXT_2]]", datos_Catalogo_descripcion2_detalle)
                            .Replace("[[PRODUCT_DETAILS_TEXT_3]]", datos_Catalogo_descripcion3_detalle);

                        var originalParent = DocumentProduct.QuerySelector("[id='product-thumbs-wrap']");
                        string replaceNew = originalParent.InnerHtml.Trim();
                        htmlChildren = htmlChildren.Replace(replaceNew, "[[PRODUCT_HTML]]");

                        var TextAllHtmlProduct = "";

                        var TextHtmlProduct = originalParent.InnerHtml;

                        var ClonTextHtmlProduct = TextHtmlProduct;
                        ClonTextHtmlProduct = ClonTextHtmlProduct.Replace("[[PRODUCT_IMAGE]]", datos_variaciones_url1_imagen_sku);
                        ClonTextHtmlProduct = ClonTextHtmlProduct.Replace("[[PRODUCT_SKU]]", datos_variaciones_sku);
                        ClonTextHtmlProduct = ClonTextHtmlProduct.Replace("[[PRODUCT_COUNT]]", "1");

                        TextAllHtmlProduct += ClonTextHtmlProduct;

                        ClonTextHtmlProduct = TextHtmlProduct;
                        ClonTextHtmlProduct = ClonTextHtmlProduct.Replace("[[PRODUCT_IMAGE]]", datos_variaciones_url2_imagen_sku);
                        ClonTextHtmlProduct = ClonTextHtmlProduct.Replace("[[PRODUCT_SKU]]", datos_variaciones_sku);
                        ClonTextHtmlProduct = ClonTextHtmlProduct.Replace("[[PRODUCT_COUNT]]", "2");

                        TextAllHtmlProduct += ClonTextHtmlProduct;

                        ClonTextHtmlProduct = TextHtmlProduct;
                        ClonTextHtmlProduct = ClonTextHtmlProduct.Replace("[[PRODUCT_IMAGE]]", datos_variaciones_url3_imagen_sku);
                        ClonTextHtmlProduct = ClonTextHtmlProduct.Replace("[[PRODUCT_SKU]]", datos_variaciones_sku);
                        ClonTextHtmlProduct = ClonTextHtmlProduct.Replace("[[PRODUCT_COUNT]]", "3");

                        TextAllHtmlProduct += ClonTextHtmlProduct;

                        ClonTextHtmlProduct = TextHtmlProduct;
                        ClonTextHtmlProduct = ClonTextHtmlProduct.Replace("[[PRODUCT_IMAGE]]", datos_variaciones_url4_imagen_sku);
                        ClonTextHtmlProduct = ClonTextHtmlProduct.Replace("[[PRODUCT_SKU]]", datos_variaciones_sku);
                        ClonTextHtmlProduct = ClonTextHtmlProduct.Replace("[[PRODUCT_COUNT]]", "4");

                        TextAllHtmlProduct += ClonTextHtmlProduct;

                        ClonTextHtmlProduct = TextHtmlProduct;
                        ClonTextHtmlProduct = ClonTextHtmlProduct.Replace("[[PRODUCT_IMAGE]]", datos_variaciones_url5_imagen_sku);
                        ClonTextHtmlProduct = ClonTextHtmlProduct.Replace("[[PRODUCT_SKU]]", datos_variaciones_sku);
                        ClonTextHtmlProduct = ClonTextHtmlProduct.Replace("[[PRODUCT_COUNT]]", "5");

                        TextAllHtmlProduct += ClonTextHtmlProduct;

                        ClonTextHtmlProduct = TextHtmlProduct;
                        ClonTextHtmlProduct = ClonTextHtmlProduct.Replace("[[PRODUCT_IMAGE]]", datos_variaciones_url6_imagen_sku);
                        ClonTextHtmlProduct = ClonTextHtmlProduct.Replace("[[PRODUCT_SKU]]", datos_variaciones_sku);
                        ClonTextHtmlProduct = ClonTextHtmlProduct.Replace("[[PRODUCT_COUNT]]", "6");

                        TextAllHtmlProduct += ClonTextHtmlProduct;

                        if (TextAllHtmlProduct != "")
                        {
                            htmlChildren = htmlChildren.Replace("[[PRODUCT_HTML]]", TextAllHtmlProduct);
                            htmlChildren = htmlChildren + "<script> var resultServer=" + Convert.ToString(result) + "</script>";
                        }
                    }
                    else if (!resultado_servidor)
                    {
                        var DocumentCatalogo = await context.OpenAsync(UrlCdnClientPlantilla + UrlDefaultTemplate + "/paginas_aplicacion/" + "process_catalogo" + ".html" + "?v=" + valueRandom);
                        if (DocumentCatalogo.StatusCode == HttpStatusCode.OK)
                        {
                            htmlChildren = DocumentCatalogo.Source.Text;
                        }
                        else
                        {
                            htmlChildren = null;
                        }
                    }
                }


                if (htmlChildren != null)
                {
                    htmlFather = htmlFather.Replace("[[HTML_CONTENT]]", htmlChildren);
                }
                else
                {
                    var documentError = await context.OpenAsync(UrlCdnClientPlantilla + UrlDefaultTemplate + "/paginas_contenido/" + "error" + ".html" + "?v=" + valueRandom);
                    var htmlError = documentError.Source.Text;
                    htmlFather = htmlFather.Replace("[[HTML_CONTENT]]", htmlError);
                }

            }
            catch (Exception ex)
            {
                var config = Configuration.Default.WithDefaultLoader();
                var context = BrowsingContext.New(config);
                var documentError = await context.OpenAsync(UrlCdnClientPlantilla + UrlDefaultTemplate + "/paginas_contenido/" + "error-tecnical" + ".html" + "?v=" + valueRandom);
                var htmlError = documentError.Source.Text;
                htmlFather = htmlFather.Replace("[[HTML_CONTENT]]", htmlError);
            }

            bool resultado_servidor_google = false;
            dynamic resultGoogle = null;
            bool resultado_servidor_header = false;
            HttpClient ClientGoogle = new HttpClient();
            var ResultGoogle = await ClientGoogle.GetAsync(UrlGoogleStorage + "/" + hostFolderClient + UrlDefaultTemplate + "/json/Scripts_de_seguimiento.json?v=" + valueRandom);
            var ContentGoogle = ResultGoogle.Content.ReadAsStringAsync().Result;
            if (ResultGoogle.StatusCode == HttpStatusCode.OK)
            {
                resultGoogle = JsonConvert.DeserializeObject(ContentGoogle);
                resultado_servidor_google = true;
            }

            if (resultado_servidor_google)
            {
                var tipo_tag_g = (string)resultGoogle.obj.Google_analytics[1].tipo_tag;
                var estado_g = (string)resultGoogle.obj.Google_analytics[2].estado;
                var id_facebook = resultGoogle.obj.Pixel_facebook[0].ID_pixel;
                var estado_tag_f = resultGoogle.obj.Pixel_facebook[1].estado;
                var result_obj_google = (string)resultGoogle.obj.Google_analytics[0].ID_seguimiento;
                var script = "<script class=\"tag_google_facebook\" async src=\"https://www.googletagmanager.com/gtag/js?id=" + result_obj_google + "\"></script>\r\n<script  class=\"tag_google_facebook\">\r\n  window.dataLayer = window.dataLayer || [];\r\n  function gtag(){dataLayer.push(arguments);}\r\n  gtag('js', new Date());\r\n\r\n  gtag('config','" + result_obj_google + "');\r\n</script>";
                var script_body = "<!-- Google Tag Manager (noscript) -->\r\n<noscript><iframe src=\"https://www.googletagmanager.com/ns.html?id=" + result_obj_google + " height=\"0\" width=\"0\" style=\"display:none;visibility:hidden\"></iframe></noscript>";

                if (estado_g == "Activado")
                {
                    if (tipo_tag_g == "2")
                    {
                        script = "<script class=\"tag_google_facebook\">(function(w, d, s, l, i){w[l]=w[l]||[];w[l].push({'gtm.start':new Date().getTime(),event:'gtm.js'});var f = d.getElementsByTagName(s)[0],j = d.createElement(s), dl = l != 'dataLayer' ? '&l=' + l : ''; j.async=true;j.src='https://www.googletagmanager.com/gtm.js?id='+i+dl;f.parentNode.insertBefore(j, f);})(window, document,'script','dataLayer','" + result_obj_google + "');</script><!-- End Google Tag Manager -->";
                        htmlFather = htmlFather.Replace("[[TAG_2_GOOGLE]]", script);
                        htmlFather = htmlFather.Replace("[[TAG_1_GOOGLE]]", "");
                        htmlFather = htmlFather.Replace("[[TAG_GM_NOSCRIPT]]", script_body);
                    }
                    else
                    {
                        htmlFather = htmlFather.Replace("[[TAG_1_GOOGLE]]", script);
                        htmlFather = htmlFather.Replace("[[TAG_2_GOOGLE]]", "");
                        htmlFather = htmlFather.Replace("[[TAG_GM_NOSCRIPT]]", "");
                    }
                }

                var script_f = "<script>!function(f, b, e, v, n, t, s){if (f.fbq) return; n = f.fbq = function(){n.callMethod?n.callMethod.apply(n, arguments):n.queue.push(arguments)};if (!f._fbq) f._fbq = n; n.push = n; n.loaded = !0; n.version = '2.0';n.queue =[]; t = b.createElement(e); t.async = !0;t.src = v; s = b.getElementsByTagName(e)[0];s.parentNode.insertBefore(t, s)}(window, document, 'script','https://connect.facebook.net/en_US/fbevents.js');fbq('init', " + id_facebook + ");fbq('track', 'PageView');</script><noscript><img height = \"1\" width = \"1\" style = \"display:none\" src = \"https://www.facebook.com/tr?id=" + id_facebook + "&ev=PageView&noscript=1\"/></noscript> ";

                if (estado_tag_f == "Activado")
                {
                    htmlFather = htmlFather.Replace("[[TAG_PIXEL_FACEBOOK]]", script_f);
                }
            }
            dynamic resultHeader = null;
            dynamic resultCategoria = null;
            HttpClient ClientS3 = new HttpClient();
            var ResultHeader = await ClientS3.GetAsync(UrlGoogleStorage + "/" + hostFolderClient + UrlDefaultTemplate + "/json/config-store.json");
            var ResultCategoria = await ClientS3.GetAsync(UrlApiCatalog + "/datoscatalogo/" + urlName3 + "/categoria/" + urlName);
            var ContentHeader = ResultHeader.Content.ReadAsStringAsync().Result;
            var ContentCategoria = ResultCategoria.Content.ReadAsStringAsync().Result;

            // BUSQUEDAS EN CAJA DE TEXTO Y VER TODO

            var paginacontenido = false;
            var process = "";
            if (urlName.Equals("f"))
            {
                htmlFather = htmlFather.Replace("[[OG_TITULO]]", "Búsqueda: " + urlName2);
                htmlFather = htmlFather.Replace("[[OG_DESCRIPCION]]", "Búsqueda: " + urlName2);
                htmlFather = htmlFather.Replace("[[OG_PALABRAS_CLAVE]]", urlName2);
            } else {
                if (urlName.Equals("process") || urlName.Equals("registro")) {
                    if (urlName.Equals("registro")) {
                        process = "Registro";
                    } else {
                        switch (urlName2)
                        {
                            case "shopcart":
                                process = "Carrito de Compras";
                                break;
                            case "thanks":
                                process = "Gracias por su Compra";
                                break;
                            case "checkout":
                                process = "Finalizar Compra";
                                break;
                            case "confirm":
                                process = "Confirmar su Compra";
                                break;
                            case "orderfind":
                                process = "Buscar su Orden";
                                break;
                            case "orderreview":
                                process = "Revisar su Orden";
                                break;
                        }
                    }
                    paginacontenido = true;
                }
            }

            if (ResultHeader.StatusCode == HttpStatusCode.OK)
            {
                resultHeader = JsonConvert.DeserializeObject(ContentHeader);
                resultado_servidor_header = true;
            }

            if (resultado_servidor_header)
            {
                if (paginacontenido) {
                    urlName = "home";
                }
                dynamic colores = JsonConvert.DeserializeObject(hexPaletaColores);

                string meta_titulo = resultHeader[0].ss_nombre_tienda;
                string meta_imagen = resultHeader[0].ss_url_logo_head;
                string meta_description = resultHeader[0].ss_descripcion_tienda;

                string aux_meta_color = resultHeader[0].ss_color;
                if (aux_meta_color == "" || aux_meta_color == null) {
                    aux_meta_color = "green";
                }
               
                htmlFather = htmlFather
                    .Replace("[[COLOR1]]", (string)colores[aux_meta_color].color1)
                    .Replace("[[COLOR2]]", (string)colores[aux_meta_color].color2)
                    .Replace("[[COLOR3]]", (string)colores[aux_meta_color].color3)
                    .Replace("[[COLOR4]]", (string)colores[aux_meta_color].color4)
                    .Replace("[[OG_TITULO]]", paginacontenido ? process : meta_titulo)
                    .Replace("[[OG_DESCRIPCION]]", meta_description)
                    .Replace("[[OG_PALABRAS_CLAVE]]", "")
                    .Replace("[[OG_PRINCIPAL_IMAGE]]", meta_imagen);
            }
            htmlFather = htmlFather
                .Replace("[[URL_CDN_STORAGE]]", UrlCdnClient + UrlDefaultTemplate)
                .Replace("[[URL_CDN_STORAGE_PRINCIPAL]]", UrlCdnClientPrincipal)
                .Replace("[[URL_CDN_STORAGE_PRIVADO]]", UrlCdnClient)
                .Replace("[[URL_DOMAIN_DEFAULT]]", urlName3)
                .Replace("[[VERSION_URL]]", Convert.ToString(valueRandom));
            htmlFather = htmlFather.Replace("[[OG_URL]]", "https://" + urlName3 + "/" + urlName);
            return new ContentResult
            {
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK,
                Content = htmlFather
            };
        }
    }
}