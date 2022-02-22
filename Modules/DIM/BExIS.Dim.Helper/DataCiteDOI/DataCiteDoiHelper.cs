﻿using BExIS.Dim.Entities.Mapping;
using BExIS.Dim.Helpers.Mapping;
using BExIS.Dlm.Entities.Data;
using BExIS.Xml.Helpers;
using System;
using System.Web;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Web.Mvc;
using System.Configuration;
using System.Threading;
using BExIS.Security.Services.Utilities;
using RestSharp;
using RestSharp.Authenticators;
using Lucifron.ReST.Models;
using Newtonsoft.Json.Linq;

namespace BExIS.Dim.Helpers
{
    public class DataCiteDoiHelper
    {
        private string getDOI(long id, string token)
        {
            var client = new RestClient(ConfigurationManager.AppSettings["lucifron"]);
            client.Authenticator = new JwtAuthenticator(token);

            var request = new RestRequest($"api/dois/{id}", Method.GET);
            var response = client.Execute(request);

            JObject joResponse = JObject.Parse(response.Content);
            string doi = joResponse["doi"].ToString();

            return response.Content;
        }

        private bool sendMetadata(DatasetVersion datasetVersion, string datasetUrl, long version, string doi, string token)
        {
            //
            // authors
            var authors = MappingUtils.GetValuesFromMetadata((int)Key.Author, LinkElementType.Key, datasetVersion.Dataset.MetadataStructure.Id, XmlUtility.ToXDocument(datasetVersion.Metadata));

            //
            // titles
            var titles = MappingUtils.GetValuesFromMetadata((int)Key.Title, LinkElementType.Key, datasetVersion.Dataset.MetadataStructure.Id, XmlUtility.ToXDocument(datasetVersion.Metadata));

            //
            // subjects
            var subjects = MappingUtils.GetValuesFromMetadata((int)Key.Subject, LinkElementType.Key, datasetVersion.Dataset.MetadataStructure.Id, XmlUtility.ToXDocument(datasetVersion.Metadata));

            //
            // descriptions
            var descriptions = MappingUtils.GetValuesFromMetadata((int)Key.Description, LinkElementType.Key, datasetVersion.Dataset.MetadataStructure.Id, XmlUtility.ToXDocument(datasetVersion.Metadata));

            var client = new RestClient(ConfigurationManager.AppSettings["lucifron"]);
            client.Authenticator = new JwtAuthenticator(token);

            var dataCiteModel = new DataCiteModel()
            {
                Data = new DataCiteData()
                {
                    Type = Lucifron.ReST.Models.Type.DOIs,
                    Attributes = new Attributes()
                    {
                        Creators = authors.Select(a => new DataCiteCreator() { Name = a }).ToList(),
                        Titles = titles.Select(t => new DataCiteTitle() { Title = t }).ToList(),
                        Subjects = subjects.Select(s => new DataCiteSubject() { Subject = s }).ToList(),
                        Version = $"{version}",
                        Dates = new List<DataCiteDate>() { new DataCiteDate() { Type = DataCiteDateType.Issued, Date = $"{DateTime.UtcNow.Year}" } },
                        DOI = doi,
                        Event = Event.Hide,
                        Types = new DataCiteTypes() { ResourceTypeGeneral = ResourceType.Dataset },
                        PublicationYear = DateTime.UtcNow.Year,
                        Publisher = ConfigurationManager.AppSettings["doiPublisher"],
                        URL = $"{datasetUrl}?version={version}",
                        Descriptions = descriptions.Select(d => new DataCiteDescription() { Language = null, Description = d, DataCiteDescriptionType = DataCiteDescriptionType.Abstract }).ToList()
                    }
                }
            };

            var request = new RestRequest($"api/dois", Method.POST).AddJsonBody(dataCiteModel);
            var response = client.Execute(request);

            return response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Created;
        }

        public string issueDoi(DatasetVersion datasetVersion, string datasetUrl, long versionNo)
        {
            // BExISDOIClient.BExISDOIClient doiClient = new BExISDOIClient.BExISDOIClient();
            // bool testmode = Convert.ToBoolean(ConfigurationManager.AppSettings["doiTestmode"]);
            string token = ConfigurationManager.AppSettings["doiToken"];

            // string doiProvider = doiClient.getProviderList(token).Where(t => t.ToLower().Equals(ConfigurationManager.AppSettings["doiProvider"].ToLower())).FirstOrDefault();           
            // string doi = doiClient.getDoi(doiProvider, testmode, testmode, "DS" + datasetVersion.Dataset.Id + "VN" + versionNo + "DV" + datasetVersion.Id, token);
            // string url = datasetUrl + "?version=" + versionNo;

            //if (testmode)
            // {
            //     postMetadata(datasetVersion, doiClient, doiProvider, doi, versionNo, token, testmode);
            //     Thread.Sleep(5000);
            //     doi = doiClient.postDoi(doiProvider, testmode, testmode, url, doi, token);
            // }
            // else
            // {
            //     doi = doiClient.postDoi(doiProvider, testmode, testmode, url, doi, token);
            //     Thread.Sleep(5000);
            //     postMetadata(datasetVersion, doiClient, doiProvider, doi, versionNo, token, testmode);
            // }

            // return doi;

            var doi_response = getDOI(datasetVersion.Dataset.Id, token);

            JObject joResponse = JObject.Parse(doi_response);
            string doi = joResponse["doi"].ToString();

            var response = sendMetadata(datasetVersion, datasetUrl, versionNo, doi, token);

            if (response)
                return doi;
            return null;
        }
        private string postMetadata(DatasetVersion datasetVersion, BExISDOIClient.BExISDOIClient doiClient, string doiProvider, string doi, long versionNo, string token, bool testmode = true)
        {
            XmlDocument doiMetadata = new XmlDocument();
            List<string> tmp = null;

            XmlNode header = doiMetadata.CreateXmlDeclaration("1.0", "UTF-8", null);
            doiMetadata.PrependChild(header);

            XmlElement resource = doiMetadata.CreateElement("resource");
            resource.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            resource.SetAttribute("xmlns", "http://datacite.org/schema/kernel-4");
            resource.SetAttribute("xsi:schemaLocation", "http://www.w3.org/2001/XMLSchema-instance", "http://datacite.org/schema/kernel-4 http://schema.datacite.org/meta/kernel-4/metadata.xsd");
            doiMetadata.AppendChild(resource);

            XmlElement identifier = doiMetadata.CreateElement("identifier");
            identifier.SetAttribute("identifierType", "DOI");
            identifier.InnerText = doi;
            resource.AppendChild(identifier);

            tmp = new List<string>();
            tmp = MappingUtils.GetValuesFromMetadata((int)Key.Author, LinkElementType.Key, datasetVersion.Dataset.MetadataStructure.Id, XmlUtility.ToXDocument(datasetVersion.Metadata));

            if (tmp.Count > 0)
            {
                XmlElement creators = doiMetadata.CreateElement("creators");
                resource.AppendChild(creators);

                foreach (string s in tmp)
                {
                    if (!String.IsNullOrEmpty(s))
                    {
                        XmlElement creator = doiMetadata.CreateElement("creator");
                        XmlElement creatorName = doiMetadata.CreateElement("creatorName");
                        creatorName.InnerText = s;
                        creator.AppendChild(creatorName);
                        creators.AppendChild(creator);
                    }
                }
            }

            tmp = new List<string>();
            tmp = MappingUtils.GetValuesFromMetadata((int)Key.Title, LinkElementType.Key, datasetVersion.Dataset.MetadataStructure.Id, XmlUtility.ToXDocument(datasetVersion.Metadata));

            XmlElement titles = doiMetadata.CreateElement("titles");
            resource.AppendChild(titles);

            if (tmp.Count > 0)
            {
                foreach (string s in tmp)
                {
                    if (!String.IsNullOrEmpty(s))
                    {
                        XmlElement title = doiMetadata.CreateElement("title");
                        title.InnerText = s;
                        titles.AppendChild(title);
                    }
                }
            }
            else
            {
                XmlElement title = doiMetadata.CreateElement("title");
                title.InnerText = new XmlDatasetHelper().GetInformationFromVersion(datasetVersion.Id, NameAttributeValues.title);
                titles.AppendChild(title);
            }

            XmlElement publisher = doiMetadata.CreateElement("publisher");
            publisher.InnerText = ConfigurationManager.AppSettings["doiPublisher"];
            resource.AppendChild(publisher);

            XmlElement publicationYear = doiMetadata.CreateElement("publicationYear");
            publicationYear.InnerText = DateTime.UtcNow.Year.ToString();
            resource.AppendChild(publicationYear);

            tmp = new List<string>();
            tmp = MappingUtils.GetValuesFromMetadata((int)Key.Subject, LinkElementType.Key, datasetVersion.Dataset.MetadataStructure.Id, XmlUtility.ToXDocument(datasetVersion.Metadata));

            if (tmp.Count > 0)
            {
                XmlElement subjects = doiMetadata.CreateElement("subjects");
                resource.AppendChild(subjects);

                foreach (string s in tmp)
                {
                    string[] sl = s.Split(',');
                    foreach (string ss in sl)
                    {
                        string it = ss.Trim();
                        if (!String.IsNullOrEmpty(ss))
                        {
                            XmlElement subject = doiMetadata.CreateElement("subject");
                            subject.InnerText = it;
                            subjects.AppendChild(subject);
                        }
                    }
                }
            }

            XmlElement resourceType = doiMetadata.CreateElement("resourceType");
            resourceType.SetAttribute("resourceTypeGeneral", "Dataset");
            resourceType.InnerText = "Dataset";
            resource.AppendChild(resourceType);

            XmlElement dates = doiMetadata.CreateElement("dates");
            resource.AppendChild(dates);
            XmlElement date = doiMetadata.CreateElement("date");
            date.SetAttribute("dateType", "Issued");
            date.InnerText = DateTime.UtcNow.Year.ToString();
            dates.AppendChild(date);

            XmlElement version = doiMetadata.CreateElement("version");
            version.InnerText = versionNo.ToString();
            resource.AppendChild(version);

            tmp = new List<string>();
            tmp = MappingUtils.GetValuesFromMetadata((int)Key.Description, LinkElementType.Key, datasetVersion.Dataset.MetadataStructure.Id, XmlUtility.ToXDocument(datasetVersion.Metadata));

            if (tmp.Count > 0)
            {
                XmlElement descriptions = doiMetadata.CreateElement("descriptions");
                resource.AppendChild(descriptions);

                foreach (string s in tmp)
                {
                    if (!String.IsNullOrEmpty(s))
                    {
                        XmlElement description = doiMetadata.CreateElement("description");
                        description.SetAttribute("descriptionType", "Abstract");
                        description.InnerText = s;
                        descriptions.AppendChild(description);
                    }
                }
            }

            return doiClient.postMetadata(doiProvider, testmode, testmode, doiMetadata, doi, token);
        }

        public void sendRequest(DatasetVersion datasetVersion, string datasetUrl)
        {
            EmailService es = new EmailService();
            List<string> tmp = null;
            List<string> emails = new List<string>();
            string title = new XmlDatasetHelper().GetInformationFromVersion(datasetVersion.Id, NameAttributeValues.title);
            string subject = "DOI Request for Dataset " + title + "(" + datasetVersion.Dataset.Id + ")";
            string body = "<p>A DOI was reqested for dataset <a href=\"" + datasetUrl + "\">" + title + "(" + datasetVersion.Dataset.Id + ")</a>, by " + HttpContext.Current.User.Identity.Name + ".</p>";

            tmp = new List<string>();
            tmp = MappingUtils.GetValuesFromMetadata((int)Key.Email, LinkElementType.Key, datasetVersion.Dataset.MetadataStructure.Id, XmlUtility.ToXDocument(datasetVersion.Metadata));


            foreach (string s in tmp)
            {
                var email = s.Trim();
                if (!string.IsNullOrEmpty(email) && !emails.Contains(email))
                {
                    emails.Add(email);
                }

            }
            es.Send(subject, body, emails);

            es.Send(subject, body, ConfigurationManager.AppSettings["SystemEmail"]);
        }
    }
}
