﻿using System;
using System.Collections.Generic;
using DataModelLibrary;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace FileGenerator
{
    /// <summary>
    /// CSV file generation class.
    /// </summary>
    class CsvCreater : FileCreater
    {
        private StringBuilder builder;
        public CsvCreater(FileInformation fileinformation, FileType fileExtension)
            : base(fileinformation, fileExtension)
        {
            builder = new StringBuilder();
        }

        object block = new object();

        /// <summary>
        /// Method, which creates a file asynchronously.
        /// </summary>
        public async override void CreateAsync()
        {
            await Task.Factory.StartNew(this.Create);
        }

        /// <summary>
        /// The base method for file creation. 
        /// </summary>
        public void Create()
        {
            lock (block)
            {
                List<DataModel> dataList = base.DataList;
                string fileName = base.fileinformation.FileName;
                try
                {

                    using (FileStream stream = new FileStream(base.GetFullName(fileName), FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            for (int i = 0; i < dataList.Count; i++)
                            {
                                bool worked = false;
                                string dataLine = ConcateDataModelFileds(dataList[i]);
                                string[] projectLine = new string[dataList[i].Projects.Length];
                                string currentLine = "";

                                for (int j = 0; j < dataList[i].Projects.Length; j++)
                                {
                                    projectLine[j] = ConcateProjectFields(dataList[i].Projects[j]);
                                    if (!worked)
                                    {
                                        currentLine = dataLine + projectLine[j];
                                        worked = true;
                                    }
                                    else
                                    {
                                        currentLine = new string(';', (byte)1) + projectLine[j];
                                    }

                                    writer.Write(currentLine);
                                }
                                writer.Write(Environment.NewLine);
                            }
                        }
                    }
                }
                catch (Exception excetion)
                {
                    MessageBox.Show(excetion.Message);
                }
            }
        }

        /// <summary>
        /// Concating all data for one content.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <returns></returns>
        private string ConcateDataModelFileds(DataModel dataModel)
        {
            builder.Clear();
            builder.Append(dataModel.TeamID); builder.Append(';');
            builder.Append(dataModel.TeamName); builder.Append(';');
            builder.Append(dataModel.MemberID); builder.Append(';');
            builder.Append(dataModel.PassportNumber); builder.Append(';');
            builder.Append(dataModel.MemberName); builder.Append(';');
            builder.Append(dataModel.MemberSurname); builder.Append(';');


            return builder.ToString();
        }

        /// <summary>
        /// Concats all data from the Project object.
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        private string ConcateProjectFields(Project project)
        {
            builder.Clear();
            builder.Append(project.ProjectID); builder.Append(',');
            builder.Append(project.ProjectName); builder.Append(',');
            builder.Append(project.ProjectCreatedDate); builder.Append(',');
            builder.Append(project.ProjectDueDate); builder.Append(',');
            builder.Append(project.ProjectDescription);

            return builder.ToString();
        }

    }
}
