using ScheduleVis.BO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VDS.RDF;

namespace ScheduleVis
{
    public class ThreadWrittenGraph
    {
        public ThreadWrittenGraph(string FileNameFormat, IEnumerable<IImportedItem> _itemsToWrite,long _fileNumber)
        {
            Started = false;
            Finished = false;
            fileNameFormat = FileNameFormat;
            itemsToWrite = _itemsToWrite;
            fileNumber = (long)_fileNumber;
        }

        public void Save()
        {
            Started = true;
            string fName = string.Format(fileNameFormat, fileNumber);
            int uniqueFileNumber = 0;
            while(File.Exists(fName))//The file numbers should be unique, but some kind of race hazard ensures that they are not. I should probably fix the cause of this at some point
            {
                string numberStr = fileNumber.ToString() + "_" + uniqueFileNumber++.ToString();
                fName = string.Format(fileNameFormat, numberStr);
            }
            IGraph graphToWrite = new Graph();
            graphToWrite.BaseUri = UriFactory.Create(Properties.Settings.Default.StationGraphBase);
            ontovis.Util.AddNamesSpaces(graphToWrite);
            foreach (IImportedItem element in itemsToWrite)
            {
                element.SaveToGraph(graphToWrite, null);
            }
            try
            {
                //Do a whilst file exists try new name loop here      
                graphToWrite.SaveToFile(fName);
            }
            catch
            { }
            Finished = true;
        }
        private string fileNameFormat;
        private IEnumerable<IImportedItem> itemsToWrite;
        public volatile bool Started;
        public volatile bool Finished;
        public long fileNumber;
    }
}
