﻿using BExIS.Dlm.Entities.Data;
using BExIS.Dlm.Entities.MetadataStructure;
using BExIS.Dlm.Services.Data;
using BExIS.Dlm.Services.MetadataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace BExIS.Xml.Helpers
{
    public class XmlDatasetHelper
    {
        #region get

        /// <summary>
        /// 
        /// </summary>
        /// <param name="datasetid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetInformation(long datasetid, NameAttributeValues name)
        {
            DatasetManager dm = new DatasetManager();
            Dataset dataset = dm.GetDataset(datasetid);
            DatasetVersion datasetVersion = dm.GetDatasetLatestVersion(dataset);

            return GetInformation(datasetVersion, name);
        }

        /// <summary>
        /// Information in metadata is stored as xml
        /// get back the vale of an attribute
        /// e.g. title  = "dataset title"        
        /// /// </summary>
        /// <param name="datasetVersion"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetInformation(DatasetVersion datasetVersion, NameAttributeValues name)
        {
            // get MetadataStructure 
            if (datasetVersion != null && datasetVersion.Dataset != null &&
            datasetVersion.Dataset.MetadataStructure != null && datasetVersion.Metadata != null)
            {
                MetadataStructure metadataStructure = datasetVersion.Dataset.MetadataStructure;
                XDocument xDoc = XmlUtility.ToXDocument((XmlDocument)datasetVersion.Dataset.MetadataStructure.Extra);
                XElement temp = XmlUtility.GetXElementByAttribute(nodeNames.nodeRef.ToString(), "name", name.ToString(),
                    xDoc);

                string xpath = temp.Attribute("value").Value.ToString();

                XmlNode node = datasetVersion.Metadata.SelectSingleNode(xpath);

                string title = "";
                if (node != null)
                    title = datasetVersion.Metadata.SelectSingleNode(xpath).InnerText;

                return title;
            }
            return string.Empty;
        }

        /// <summary>
        /// Information in metadata is stored as xml
        /// get back the vale of an attribute
        /// e.g. title  = "dataset title"        
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetInformation(Dataset dataset, NameAttributeValues name)
        {
            DatasetManager dm = new DatasetManager();
            DatasetVersion datasetVersion = dm.GetDatasetLatestVersion(dataset);

            return GetInformation(datasetVersion, name);
        }

        /// <summary>
        /// Information in metadata is stored as xml
        /// get back the xpath of an attribute
        /// e.g. title  = metadata/dataset/title
        /// </summary>
        /// <param name="metadataStructure"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetInformationPath(MetadataStructure metadataStructure, NameAttributeValues name)
        {

            XDocument xDoc = XmlUtility.ToXDocument((XmlDocument)metadataStructure.Extra);
            XElement temp = XmlUtility.GetXElementByAttribute(nodeNames.nodeRef.ToString(), "name", name.ToString(),
                xDoc);

            string xpath = temp.Attribute("value").Value.ToString();

            return xpath;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="datasetid"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetTransmissionInformation(long datasetid, TransmissionType type)
        {
            DatasetManager dm = new DatasetManager();
            Dataset dataset = dm.GetDataset(datasetid);
            DatasetVersion datasetVersion = dm.GetDatasetLatestVersion(dataset);

            return GetTransmissionInformation(datasetVersion, type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="datasetVersion"></param>
        /// <param name="type"></param>
        /// <param name="returnType"></param>
        /// <returns></returns>
        public static string GetTransmissionInformation(DatasetVersion datasetVersion, TransmissionType type,
            AttributeNames returnType = AttributeNames.value)
        {
            // get MetadataStructure 
            if (datasetVersion != null && datasetVersion.Dataset != null &&
                datasetVersion.Dataset.MetadataStructure != null && datasetVersion.Metadata != null)
            {
                MetadataStructure metadataStructure = datasetVersion.Dataset.MetadataStructure;
                XDocument xDoc = XmlUtility.ToXDocument((XmlDocument)datasetVersion.Dataset.MetadataStructure.Extra);
                IEnumerable<XElement> temp = XmlUtility.GetXElementsByAttribute(nodeNames.convertRef.ToString(), "type",
                    type.ToString(), xDoc);

                string value = temp.First().Attribute(returnType.ToString()).Value;

                return value;
            }
            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="datasetVersion"></param>
        /// <param name="field"></param>
        /// <param name="fieldValue"></param>
        /// <param name="returnType"></param>
        /// <returns></returns>
        public static string GetTransmissionInformation(DatasetVersion datasetVersion, AttributeNames field, string fieldValue,
            AttributeNames returnType = AttributeNames.value)
        {
            // get MetadataStructure 
            if (datasetVersion != null && datasetVersion.Dataset != null &&
                datasetVersion.Dataset.MetadataStructure != null && datasetVersion.Metadata != null)
            {
                MetadataStructure metadataStructure = datasetVersion.Dataset.MetadataStructure;
                XDocument xDoc = XmlUtility.ToXDocument((XmlDocument)datasetVersion.Dataset.MetadataStructure.Extra);
                IEnumerable<XElement> temp = XmlUtility.GetXElementsByAttribute(nodeNames.convertRef.ToString(), field.ToString(),
                    fieldValue, xDoc);

                string value = temp.First().Attribute(returnType.ToString()).Value;

                return value;
            }
            return string.Empty;
        }

        public static string GetTransmissionInformation(DatasetVersion datasetVersion, TransmissionType type, string name,
            AttributeNames returnType = AttributeNames.value)
        {
            // get MetadataStructure 
            if (datasetVersion != null && datasetVersion.Dataset != null &&
                datasetVersion.Dataset.MetadataStructure != null && datasetVersion.Metadata != null)
            {
                MetadataStructure metadataStructure = datasetVersion.Dataset.MetadataStructure;
                XDocument xDoc = XmlUtility.ToXDocument((XmlDocument)datasetVersion.Dataset.MetadataStructure.Extra);

                Dictionary<string, string> queryDic = new Dictionary<string, string>();
                queryDic.Add(AttributeNames.name.ToString(), name);
                queryDic.Add(AttributeNames.type.ToString(), type.ToString());

                IEnumerable<XElement> temp = XmlUtility.GetXElementsByAttribute(nodeNames.convertRef.ToString(), queryDic, xDoc);

                string value = temp.First().Attribute(returnType.ToString()).Value;

                return value;
            }
            return string.Empty;
        }

        public static bool HasImportInformation(long metadataStructrueId)
        {
            // get MetadataStructure 
            MetadataStructureManager metadataStructureManager = new MetadataStructureManager();
            MetadataStructure metadataStructure = metadataStructureManager.Repo.Get(metadataStructrueId);

            XDocument xDoc = XmlUtility.ToXDocument((XmlDocument)metadataStructure.Extra);
            IEnumerable<XElement> tmp = XmlUtility.GetXElementsByAttribute(nodeNames.convertRef.ToString(), AttributeNames.type.ToString(),
                TransmissionType.mappingFileImport.ToString(), xDoc);

            if (tmp.Any()) return true;

            return false;
        }

        public static bool HasExportInformation(long metadataStructrueId)
        {
            // get MetadataStructure 
            MetadataStructureManager metadataStructureManager = new MetadataStructureManager();
            MetadataStructure metadataStructure = metadataStructureManager.Repo.Get(metadataStructrueId);

            XDocument xDoc = XmlUtility.ToXDocument((XmlDocument)metadataStructure.Extra);
            IEnumerable<XElement> tmp = XmlUtility.GetXElementsByAttribute(nodeNames.convertRef.ToString(), AttributeNames.type.ToString(),
                TransmissionType.mappingFileExport.ToString(), xDoc);

            if (tmp.Any()) return true;

            return false;

        }

        /// <summary>
        /// returns a value of a metadata node
        /// </summary>
        /// <param name="datasetVersion"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetAllTransmissionInformation(long datasetid, TransmissionType type,
            AttributeNames returnType = AttributeNames.value)
        {
            DatasetManager dm = new DatasetManager();
            Dataset dataset = dm.GetDataset(datasetid);
            DatasetVersion datasetVersion = dm.GetDatasetLatestVersion(dataset);

            // get MetadataStructure 
            if (datasetVersion != null && datasetVersion.Dataset != null &&
                datasetVersion.Dataset.MetadataStructure != null && datasetVersion.Metadata != null)
            {
                return GetAllTransmissionInformationFromMetadataStructure(datasetVersion.Dataset.MetadataStructure.Id,
                    type, returnType);
            }
            return null;
        }

        /// <summary>
        /// returns a List of all transmission nodes in the metadataStructure
        /// </summary>
        /// <param name="metadatastrutcureId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetAllTransmissionInformationFromMetadataStructure(long metadatastrutcureId, TransmissionType type,
            AttributeNames returnType = AttributeNames.value)
        {

            MetadataStructureManager metadataStructureManager = new MetadataStructureManager();
            MetadataStructure metadataStructure = metadataStructureManager.Repo.Get(metadatastrutcureId);

            List<string> tmpList = new List<string>();

            try
            {
                XDocument xDoc = XmlUtility.ToXDocument((XmlDocument)metadataStructure.Extra);
                IEnumerable<XElement> temp = XmlUtility.GetXElementsByAttribute(nodeNames.convertRef.ToString(), AttributeNames.type.ToString(),
                    type.ToString(), xDoc);

                foreach (var element in temp)
                {
                    tmpList.Add(element.Attribute(returnType.ToString()).Value);
                }
            }
            catch (Exception)
            {

                return new List<string>();
            }

            return tmpList;
        }

        public static bool IsActive(long metadataStructrueId)
        {
            // get MetadataStructure 
            MetadataStructureManager metadataStructureManager = new MetadataStructureManager();
            MetadataStructure metadataStructure = metadataStructureManager.Repo.Get(metadataStructrueId);

            XDocument xDoc = XmlUtility.ToXDocument((XmlDocument)metadataStructure.Extra);
            XElement tmp = XmlUtility.GetXElementsByAttribute(nodeNames.parameter.ToString(), AttributeNames.name.ToString(),
                NameAttributeValues.active.ToString(), xDoc).FirstOrDefault();

            if (tmp != null)
            {
                try
                {
                    return Convert.ToBoolean(tmp.Attribute(AttributeNames.value.ToString()).Value);
                }
                catch (Exception)
                {

                    return false;
                }
            }

            return false;
        }

        public static bool HasTransmission(long datasetid, TransmissionType type)
        {
            DatasetManager dm = new DatasetManager();
            Dataset dataset = dm.GetDataset(datasetid);
            DatasetVersion datasetVersion = dm.GetDatasetLatestVersion(dataset);

            // get MetadataStructure 
            if (datasetVersion != null && datasetVersion.Dataset != null &&
                datasetVersion.Dataset.MetadataStructure != null && datasetVersion.Metadata != null)
            {
                MetadataStructure metadataStructure = datasetVersion.Dataset.MetadataStructure;
                XDocument xDoc = XmlUtility.ToXDocument((XmlDocument)datasetVersion.Dataset.MetadataStructure.Extra);
                IEnumerable<XElement> temp = XmlUtility.GetXElementsByAttribute(nodeNames.convertRef.ToString(), AttributeNames.type.ToString(),
                    type.ToString(), xDoc);

                if (temp != null && temp.Any()) return true;

            }
            return false;
        }

        public static bool HasMetadataStructureTransmission(long metadataStructureId, TransmissionType type)
        {
            MetadataStructureManager metadataStructureManager = new MetadataStructureManager();
            MetadataStructure metadataStructure = metadataStructureManager.Repo.Get(metadataStructureId);

            // get MetadataStructure 
            if (metadataStructure != null && metadataStructure.Extra != null)
            {

                XDocument xDoc = XmlUtility.ToXDocument((XmlDocument)metadataStructure.Extra);
                IEnumerable<XElement> temp = XmlUtility.GetXElementsByAttribute(nodeNames.convertRef.ToString(), AttributeNames.type.ToString(),
                    type.ToString(), xDoc);

                if (temp != null && temp.Any()) return true;

            }
            return false;
        }


        //todo entity extention
        public static string GetEntityType(long datasetid)
        {
            DatasetManager datasetManager = new DatasetManager();
            Dataset dataset = datasetManager.GetDataset(datasetid);

            // get MetadataStructure 
            if (dataset != null)
            {
                return GetEntityTypeFromMetadatStructure(dataset.MetadataStructure.Id);
            }
            return string.Empty;
        }

        //todo entity extention
        public static string GetEntityTypeFromMetadatStructure(long metadataStuctrueId)
        {

            MetadataStructureManager metadataStructureManager = new MetadataStructureManager();
            MetadataStructure metadataStructure = metadataStructureManager.Repo.Get(metadataStuctrueId);

            // get MetadataStructure 
            if (metadataStructure != null)
            {
                XDocument xDoc = XmlUtility.ToXDocument((XmlDocument)metadataStructure.Extra);
                IEnumerable<XElement> tmp = XmlUtility.GetXElementByNodeName(nodeNames.entity.ToString(), xDoc);
                if (tmp.Any())
                    return tmp.First().Attribute("value").Value;
            }


            return string.Empty;
        }

        //todo entity extention
        public static string GetEntityNameFromMetadatStructure(long metadataStuctrueId)
        {

            MetadataStructureManager metadataStructureManager = new MetadataStructureManager();
            MetadataStructure metadataStructure = metadataStructureManager.Repo.Get(metadataStuctrueId);

            // get MetadataStructure 
            if (metadataStructure != null)
            {
                XDocument xDoc = XmlUtility.ToXDocument((XmlDocument)metadataStructure.Extra);
                IEnumerable<XElement> tmp = XmlUtility.GetXElementByNodeName(nodeNames.entity.ToString(), xDoc);
                if (tmp.Any())
                    return tmp.First().Attribute("name").Value;
            }


            return string.Empty;
        }

        //todo entity extention
        public static bool HasEntityType(long metadataStuctrueId, string entityClassPath)
        {
            MetadataStructureManager metadataStructureManager = new MetadataStructureManager();
            MetadataStructure metadataStructure = metadataStructureManager.Repo.Get(metadataStuctrueId);

            // get MetadataStructure 
            if (metadataStructure != null)
            {
                XDocument xDoc = XmlUtility.ToXDocument((XmlDocument)metadataStructure.Extra);
                IEnumerable<XElement> tmp = XmlUtility.GetXElementByNodeName(nodeNames.entity.ToString(), xDoc);
                if (tmp.Any())
                {
                    foreach (var entity in tmp)
                    {
                        string tmpEntityClassPath = "";
                        if (entity.HasAttributes && entity.Attribute("value") != null)
                            tmpEntityClassPath = entity.Attribute("value").Value.ToLower();

                        if (tmpEntityClassPath.Equals(entityClassPath.ToLower())) return true;
                    }
                }
            }
            return false;
        }

        #region SystemReferences

        public static string GetSystemInformationPath(long metadatastructureId, SystemNameAttributeValues name)
        {
            MetadataStructureManager metadataStructureManager = new MetadataStructureManager();
            MetadataStructure metadataStructure = metadataStructureManager.Repo.Get(metadatastructureId);

            XDocument xDoc = XmlUtility.ToXDocument((XmlDocument)metadataStructure.Extra);
            XElement temp = XmlUtility.GetXElementByAttribute(nodeNames.systemRef.ToString(), "name", name.ToString(),
                xDoc);

            string xpath = "";
            if (temp != null)
                xpath = temp.Attribute("value").Value.ToString();

            return xpath;
        }

        /// <summary>
        /// Return a List of all xpaths in a metadata structure
        /// where system references existing
        /// </summary>
        /// <param name="metadatastructureId"></param>
        /// <returns></returns>
        public static List<string> GetSystemReferenceXPaths(long metadatastructureId)
        {
            List<string> tmp = new List<string>();

            MetadataStructureManager metadataStructureManager = new MetadataStructureManager();
            MetadataStructure metadataStructure = metadataStructureManager.Repo.Get(metadatastructureId);

            XDocument xDoc = XmlUtility.ToXDocument((XmlDocument)metadataStructure.Extra);
            IEnumerable<XElement> tempNodes = XmlUtility.GetXElementByNodeName(nodeNames.systemRef.ToString(), xDoc);

            foreach (var tempNode in tempNodes)
            {
                tmp.Add(tempNode.Attribute("value").Value);
            }

            return tmp;
        }
        /// <summary>
        /// Update the conent in a metadata xml document
        /// based on the system refenreces of the metadata structure
        /// </summary>
        /// <param name="metadataStructureId"></param>
        /// <param name="metadata"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static XmlDocument UpdateSystemInformationsInMetadata(long metadataStructureId, XmlDocument metadata,
            SystemMode mode, long datasetId, long datasetVersionId)
        {

            MetadataStructureManager metadataStructureManager = new MetadataStructureManager();
            MetadataStructure metadataStructure = metadataStructureManager.Repo.Get(metadataStructureId);

            XDocument xDoc = XmlUtility.ToXDocument((XmlDocument)metadataStructure.Extra);

            IEnumerable<XElement> tempNodes = null;
            // create -> all infor need to set
            if (mode.Equals(SystemMode.create))
            {
                //get all system ref from extra xml
                tempNodes = XmlUtility.GetXElementByNodeName(nodeNames.systemRef.ToString(), xDoc);

            }
            else
            {

                if (mode.Equals(SystemMode.update))
                {
                    tempNodes = XmlUtility.GetXElementsByAttribute(nodeNames.systemRef.ToString(), SystemAttributeNames.mode.ToString(), SystemMode.update.ToString(), xDoc);
                }

            }

            if (tempNodes != null)
            {
                XmlElement tmp;
                foreach (var tempnode in tempNodes)
                {
                    string infoFrom = tempnode.Attribute(SystemAttributeNames.infoFrom.ToString()).Value;
                    string xpath = tempnode.Attribute(AttributeNames.value.ToString()).Value;
                    tmp = (XmlElement)metadata.SelectSingleNode(xpath);


                    SystemInfoFrom selectedEnum = (SystemInfoFrom)Enum.Parse(typeof(SystemInfoFrom), infoFrom);

                    if (tmp != null)
                    {
                        switch (selectedEnum)
                        {
                            case SystemInfoFrom.DatasetId:
                                {
                                    tmp.InnerText = datasetId.ToString();
                                    break;
                                }
                            case SystemInfoFrom.DateNow:
                                {
                                    tmp.InnerText = DateTime.Now.ToString();
                                    break;
                                }
                            case SystemInfoFrom.VersionNumber:
                                {
                                    tmp.InnerText = datasetVersionId.ToString();
                                    break;
                                }
                            default:
                                break;


                        }
                    }

                }
            }



            return metadata;
        }

        #endregion



        #endregion

        #region add

        public static XmlDocument AddReferenceToXml(XmlDocument Source, string nodeName, string nodeValue, string nodeType, string destinationPath)
        {

            //XmlDocument doc = new XmlDocument();
            XmlNode extra;
            if (Source != null)
            {
                if (Source.DocumentElement == null)
                {
                    extra = Source.CreateElement("extra", "");
                    Source.AppendChild(extra);
                }
            }

            XmlNode x = createMissingNodes(destinationPath, Source.DocumentElement, Source, nodeName);

            //check attrviute of the xmlnode
            if (x.Attributes.Count > 0)
            {
                foreach (XmlAttribute attr in x.Attributes)
                {
                    if (attr.Name == "name") attr.Value = nodeName;
                    if (attr.Name == "value") attr.Value = nodeValue;
                    if (attr.Name == "type") attr.Value = nodeType;
                }
            }
            else
            {
                XmlAttribute name = Source.CreateAttribute("name");
                name.Value = nodeName;
                XmlAttribute value = Source.CreateAttribute("value");
                value.Value = nodeValue;
                XmlAttribute type = Source.CreateAttribute("type");
                type.Value = nodeType;

                x.Attributes.Append(name);
                x.Attributes.Append(value);
                x.Attributes.Append(type);

            }

            return Source;

        }

        public static XmlDocument AddSystemReferenceToXml(XmlDocument Source, SystemNameAttributeValues nodeName, string nodeValue)
        {

            //XmlDocument doc = new XmlDocument();
            XmlNode extra;
            if (Source != null)
            {
                if (Source.DocumentElement == null)
                {
                    extra = Source.CreateElement("extra", "");
                    Source.AppendChild(extra);
                }
            }

            XmlNode x = createMissingNodes("extra/systemReferences/systemRef", Source.DocumentElement, Source, "systemReference");

            //check attrviute of the xmlnode
            if (x.Attributes.Count > 0)
            {
                foreach (XmlAttribute attr in x.Attributes)
                {
                    if (attr.Name == "name") attr.Value = nodeName.ToString();
                    if (attr.Name == "value") attr.Value = nodeValue;
                    if (attr.Name == "type") attr.Value = "xPath";
                    if (attr.Name == "mode") attr.Value = getMode(nodeName).ToString();
                    if (attr.Name == "infoFrom") attr.Value = getInfoFrom(nodeName).ToString();
                }
            }
            else
            {
                XmlAttribute name = Source.CreateAttribute("name");
                name.Value = nodeName.ToString();
                XmlAttribute value = Source.CreateAttribute("value");
                value.Value = nodeValue;
                XmlAttribute type = Source.CreateAttribute("type");
                type.Value = "xPath";
                XmlAttribute mode = Source.CreateAttribute("mode");
                mode.Value = getMode(nodeName).ToString();
                XmlAttribute infoFrom = Source.CreateAttribute("infoFrom");
                infoFrom.Value = getInfoFrom(nodeName).ToString();

                x.Attributes.Append(name);
                x.Attributes.Append(value);
                x.Attributes.Append(type);
                x.Attributes.Append(mode);
                x.Attributes.Append(infoFrom);

            }

            return Source;

        }

        private static SystemMode getMode(SystemNameAttributeValues name)
        {
            if (name.Equals(SystemNameAttributeValues.Id) ||
                name.Equals(SystemNameAttributeValues.CreationDate))
                return SystemMode.create;

            if (name.Equals(SystemNameAttributeValues.VersionNr) ||
                name.Equals(SystemNameAttributeValues.LastDateModified))
                return SystemMode.update;

            return SystemMode.create;
        }

        private static SystemInfoFrom getInfoFrom(SystemNameAttributeValues name)
        {
            if (name.Equals(SystemNameAttributeValues.Id))
                return SystemInfoFrom.DatasetId;

            if (name.Equals(SystemNameAttributeValues.VersionNr))
                return SystemInfoFrom.VersionNumber;

            if (name.Equals(SystemNameAttributeValues.CreationDate))
                return SystemInfoFrom.DateNow;

            if (name.Equals(SystemNameAttributeValues.LastDateModified))
                return SystemInfoFrom.DateNow;


            return SystemInfoFrom.DatasetId;
        }

        private static XmlNode createMissingNodes(string destinationParentXPath, XmlNode parentNode, XmlDocument doc,
            string name)
        {
            string dif = destinationParentXPath;

            List<string> temp = dif.Split('/').ToList();
            temp.RemoveAt(0);

            XmlNode parentTemp = parentNode;

            foreach (string s in temp)
            {
                if (XmlUtility.GetXmlNodeByName(parentTemp, s) == null)
                {
                    XmlNode t = XmlUtility.CreateNode(s, doc);

                    parentTemp.AppendChild(t);
                    parentTemp = t;
                }
                else
                {
                    XmlNode t = XmlUtility.GetXmlNodeByName(parentTemp, s);

                    if (temp.Last().Equals(s))
                    {
                        if (!t.Attributes["name"].Equals(name))
                        {
                            t = XmlUtility.CreateNode(s, doc);
                            parentTemp.AppendChild(t);
                        }

                    }

                    parentTemp = t;
                }
            }

            return parentTemp;
        }

        #endregion

    }

    public enum nodeNames
    {
        nodeRef,
        convertRef,
        entity,
        parameter,
        systemRef
    }

    public enum NameAttributeValues
    {
        title,
        description,
        active
    }

    public enum AttributeNames
    {
        name,
        value,
        type,
    }

    public enum AttributeType
    {
        xpath,
        entity,
        parameter
    }

    public enum TransmissionType
    {
        mappingFileExport,
        mappingFileImport
    }

    #region PartyReference

    public enum PartyNameAttributeValues
    {
        name,
        email
    }

    public enum PartyTypes
    {
        person
    }

    #endregion

    #region RoleReference

    public enum RoleNameAttributeValues
    {
        DataRequestManager,
        NotificationReciever,
        Owner
    }

    #endregion

    #region SystemReference

    public enum SystemAttributeNames
    {
        name,
        value,
        type,
        mode,
        infoFrom
    }

    public enum SystemNameAttributeValues
    {
        Id,
        VersionNr,
        CreationDate,
        LastDateModified
    }

    public enum SystemMode
    {
        create,
        update
    }

    public enum SystemInfoFrom
    {
        DatasetId,
        VersionNumber,
        DateNow
    }

    #endregion

}
