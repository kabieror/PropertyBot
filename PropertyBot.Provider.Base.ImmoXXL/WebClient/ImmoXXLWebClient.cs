﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using PropertyBot.Common;
using PropertyBot.Provider.Base.ImmoXXL.Entity;

namespace PropertyBot.Provider.Base.ImmoXXL.WebClient
{
    internal class ImmoXXLWebClient : IImmoXXLWebClient
    {
        private const int PageItemCount = 6;

        private readonly HttpClient _client;

        public ImmoXXLWebClient()
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<IEnumerable<ImmoXXLmmoProperty>> GetObjects(ImmoXXLWebClientOptions options)
        {
            List<ImmoXXLmmoProperty> pageProperties;
            List<ImmoXXLmmoProperty> allProperties = new List<ImmoXXLmmoProperty>();
            var currentPage = 0;
            
            do
            {
                var page = await GetRawPage(options, currentPage++);
                pageProperties = ParseRawPage(page, options.BaseUri).ToList();
                allProperties.AddRange(pageProperties);
            } while (pageProperties.Count != 0);
            
            return allProperties;
        }

        private async Task<string> GetRawPage(ImmoXXLWebClientOptions options, int page)
        {
            var cursor = page * PageItemCount;
            return await _client.GetStringAsync(
                    $"{options.BaseUri}/index.php4?cmd=searchResults&alias=suchmaske&kaufartids={options.BuyIds}&kategorieids={options.CategoryIds}&objq[cursor]={cursor}");
        }

        private IEnumerable<ImmoXXLmmoProperty> ParseRawPage(string content, string baseUrl)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(content);
            var objects = htmlDoc.DocumentNode.SelectNodes("//div[@class='objekt']") ?? new HtmlNodeCollection(htmlDoc.DocumentNode);
            return objects.Select(node => ParseObject(node, baseUrl));
        }

        private ImmoXXLmmoProperty ParseObject(HtmlNode objectNode, string baseUrl)
        {
            var objectDoc = new HtmlDocument();
            objectDoc.LoadHtml(objectNode.InnerHtml);

            var id = GetId(objectDoc.DocumentNode);
            var roomCount = GetRoomCount(objectDoc.DocumentNode);
            var livingArea = GetLivingArea(objectDoc.DocumentNode);
            var propertyType = GetPropertyType(objectDoc.DocumentNode);
            var description = GetDescription(objectDoc.DocumentNode);
            var location = GetLocation(objectDoc.DocumentNode);
            var price = GetPrice(objectDoc.DocumentNode);
            var imageUrl = GetImageUrl(objectDoc.DocumentNode, baseUrl);
            var detailUrl = GetDetailUrl(objectDoc.DocumentNode, baseUrl);

            return new ImmoXXLmmoProperty(id, roomCount, livingArea, propertyType, description, location, price, imageUrl, detailUrl);
        }

        private string GetId(HtmlNode node)
        {
            var idNode = node.SelectSingleNode("//div[contains(@class, 'objnr')]/b");
            return idNode?.InnerText ?? "ID_NOT_FOUND";
        }

        private string GetRoomCount(HtmlNode node)
        {
            var objectInfos = GetObjectInfos(node);
            var roomNode = objectInfos?.FirstOrDefault(info => info.InnerHtml.Contains("ZIMMER"));
            return GetInfoNodeValue(roomNode, "0");
        }

        private int GetLivingArea(HtmlNode node)
        {
            var objectInfos = GetObjectInfos(node);
            var roomNode = objectInfos?.FirstOrDefault(info => info.InnerHtml.Contains("WOHNFLÄCHE"));
            return GetInfoNodeValue(roomNode, "0").Replace("m²", string.Empty).Trim().ToIntSafe();
        }

        private string GetPropertyType(HtmlNode node)
        {
            var typeNode = node.SelectSingleNode("//div[@class='objektart']");
            return typeNode?.InnerText ?? "?";
        }

        private string GetLocation(HtmlNode node)
        {
            var locationNode = node.SelectSingleNode("//div[@class='ort']");
            return locationNode?.InnerText ?? "No Location available";
        }

        private string GetDescription(HtmlNode node)
        {
            var descriptionNode = node.SelectSingleNode("//div[@class='hauptinfos']/h3/a");
            return descriptionNode?.InnerText ?? "No Description available";
        }

        private int GetPrice(HtmlNode node)
        {
            var priceNode = node.SelectSingleNode("//div[contains(@class, 'preis')]/span");
            var price = priceNode?.InnerText.Replace(",- &euro;", string.Empty).Replace(".", string.Empty) ?? "0";
            return price.ToIntSafe();
        }

        private Uri GetImageUrl(HtmlNode node, string baseUrl)
        {
            var imageNode = node.SelectSingleNode("//div[contains(@class, 'bg_image')]");
            var style = imageNode?.Attributes.FirstOrDefault(attribute => attribute.Name == "style")?.Value;

            if (style != null)
            {
                var urlMatch = Regex.Match(style, @"url\((.*)\)");

                if (urlMatch.Success)
                {
                    if (urlMatch.Groups.Count > 1)
                    {
                        var url = urlMatch.Groups[1];
                        return new Uri($"{baseUrl}/{url}");
                    }
                }
            }

            return new Uri("https://upload.wikimedia.org/wikipedia/commons/2/26/512pxIcon-sunset_photo_not_found.png");
        }

        private Uri GetDetailUrl(HtmlNode node, string baseUrl)
        {
            var detailNode = node.SelectSingleNode("//div[@class='hauptinfos']/h3/a");
            var href = detailNode?.Attributes.FirstOrDefault(attribute => attribute.Name == "href")?.Value.Replace("&amp;", "&");
            return new Uri(href != null ? $"{baseUrl}/{href}" : "https://www.link-immobilien.info");
        }

        private string GetInfoNodeValue(HtmlNode node, string defaultValue)
        {
            var value = node?.ChildNodes.FirstOrDefault(child => child.Name == "b")?.InnerText;
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }

        private HtmlNodeCollection GetObjectInfos(HtmlNode node)
        {
            return node.SelectNodes("//div[contains(@class, 'objektinfos')]/div[contains(@class, 'info')]");
        }
    }
}
