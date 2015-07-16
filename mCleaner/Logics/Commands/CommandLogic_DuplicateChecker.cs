﻿using CodeBureau;
using mCleaner.Helpers;
using mCleaner.Logics.Enumerations;
using mCleaner.Model;
using mCleaner.ViewModel;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace mCleaner.Logics.Commands
{
    public class CommandLogic_DuplicateChecker : CommandLogic_Base, iActions
    {
        #region vars

        #endregion

        #region properties
        public ViewModel_DuplicateChecker DupChecker
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ViewModel_DuplicateChecker>();
            }
        }
        #endregion

        #region commands

        #endregion
        public CommandLogic_DuplicateChecker()
        {

        }
        private static CommandLogic_DuplicateChecker _i = new CommandLogic_DuplicateChecker();
        public static CommandLogic_DuplicateChecker I { get { return _i; } }
        #region ctor

        #endregion

        #region command methods

        #endregion

        #region methods
        public void Execute(bool apply = false)
        {
            SEARCH search = (SEARCH)StringEnum.Parse(typeof(SEARCH), Action.search);

            switch (search)
            {
                case SEARCH.dupchecker_all:
                    EnqueueCustomPath(Action.path);
                    break;
            }
        }

        public void EnqueueCustomPath(string path)
        {
            // enqueue file for deletion
            Worker.I.EnqueTTD(new Model_ThingsToDelete()
            {
                FullPathName = path,
                IsWhitelisted = false,
                OverWrite = false,
                WhatKind = THINGS_TO_DELETE.system,
                command = COMMANDS.dupchecker,
                search = SEARCH.dupchecker_all,
                path = string.Empty,
                level = Action.level,
                cleaner_name = Action.parent_option.label
            });
        }

        public void ScanPath(string path)
        {
            List<string> files = FileOperations.I.GetFilesRecursive(path, null, new Action<string>((s) =>
            {
                ProgressWorker.I.EnQ("Retreiving files in: " + s);
            }));

            //Dictionary<string, List<string>> files_with_same_size = new Dictionary<string,List<string>>();
            Dictionary<long, List<string>> files_with_same_size = new Dictionary<long,List<string>>();
            ProgressWorker.I.EnQ("Checking for same file size. This may take a while"); // to minimize work load looking for duplicate files
            base.VMCleanerML.MaxProgress = files.Count;
            base.VMCleanerML.ProgressIndex = 0;
            int i = 0;
            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                if (fi.Length > 0) // do not include 0 length files.
                {
                    if (files_with_same_size.ContainsKey(fi.Length))
                    {
                        files_with_same_size[fi.Length].Add(fi.FullName);
                    }
                    else
                    {
                        files_with_same_size.Add(fi.Length, new List<string>() { fi.FullName });
                    }
                }

                base.VMCleanerML.ProgressIndex++;
            }
            //foreach (string file in files)
            //{
            //    foreach (string file2 in files)
            //    {
            //        FileInfo fi1 = new FileInfo(file);
            //        FileInfo fi2 = new FileInfo(file2);

            //        if (fi1.FullName != fi2.FullName) // do not match two the same file in same location
            //        {
            //            if (fi1.Length == fi2.Length)
            //            {
            //                if (files_with_same_size.ContainsKey(file))
            //                {
            //                    files_with_same_size[file].Add(file2);
            //                }
            //                else
            //                {
            //                    files_with_same_size.Add(file, new List<string>());
            //                    files_with_same_size[file].Add(file);
            //                    files_with_same_size[file].Add(file2);
            //                    ProgressWorker.I.EnQ("Queueing file: " + file);
            //                    base.VMCleanerML.ProgressIndex = i;
            //                }
            //            }
            //        }
            //    }
            //    i++;
            //}
            base.VMCleanerML.ProgressIndex = 0;

            ProgressWorker.I.EnQ("Please wait while hashing files. This may take a while");
            // get all the files we need to hash
            List<string> files_to_hash = new List<string>();
            foreach (long filesize in files_with_same_size.Keys)
            {
                if (files_with_same_size[filesize].Count > 1)
                {
                    files_to_hash.AddRange(files_with_same_size[filesize].ToArray());
                }

                base.VMCleanerML.ProgressIndex++;
            }

            List<string> hashed_files = new List<string>();
            base.VMCleanerML.MaxProgress = files_to_hash.Count;
            base.VMCleanerML.ProgressIndex = 0;

            //if (files_to_hash.Count / 5 > 50)
            //{
            //    List<Task> task_list = new List<Task>();
            //    int index = 0;
            //    int max = files_to_hash.Count / 5;
            //    for (i = 0; i < 5; i++)
            //    {
            //        var task = new Task(new Action(() =>
            //        {
            //            for (int j = index; j < max; j++)
            //            {
            //                if (j < files_to_hash.Count)
            //                {
            //                    string hash = FileOperations.I.HashFile(files_to_hash[j]);
            //                    ProgressWorker.I.EnQ("Hashing: " + files_to_hash[j] + " > " + hash);
            //                    hashed_files.Add(files_to_hash[j] + "|" + hash);
            //                }
            //            }
            //        }));
            //        task.Start();
            //        task_list.Add(task);
            //        index += max;
            //    }

            //    await Task.WhenAll(task_list.ToArray());
            //}
            //else
            {
                //foreach (string file in files_with_same_size.Keys)
                //{
                //    foreach (string samfilesize in files_with_same_size[file])
                //    {
                //        ProgressWorker.I.EnQ("Hashing: " + samfilesize);
                //        hashed_files.Add(samfilesize + "|" + FileOperations.I.HashFile(samfilesize));
                //    }
                //    base.VMCleanerML.ProgressIndex++;
                //}
                foreach (string filename in files_to_hash)
                {
                    try
                    {
                        string hash = FileOperations.I.HashFile(filename);
                        ProgressWorker.I.EnQ("Hashing: " + filename + " > " + hash);
                        hashed_files.Add(filename + "|" + hash);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }

                    base.VMCleanerML.ProgressIndex++;
                }
            }
            

            ProgressWorker.I.EnQ("Finalizing ...");
            Dictionary<string, List<string>> files_with_same_hash = new Dictionary<string, List<string>>();
            base.VMCleanerML.MaxProgress = hashed_files.Count;
            base.VMCleanerML.ProgressIndex = 0;

            foreach (string hashedfile in hashed_files)
            {
                string[] tmp = hashedfile.Split('|');
                string file = tmp[0];
                string hash = tmp[1];

                if (files_with_same_hash.ContainsKey(hash))
                {
                    if (!files_with_same_hash[hash].Contains(file))
                    {
                        files_with_same_hash[hash].Add(file);
                    }
                }
                else
                {
                    files_with_same_hash.Add(hash, new List<string>());
                    files_with_same_hash[hash].Add(file);
                }
                base.VMCleanerML.ProgressIndex++;
            }

            List<string> teremove = new List<string>();
            foreach (string key in files_with_same_hash.Keys)
            {
                if (files_with_same_hash[key].Count == 1) teremove.Add(key);
            }
            foreach(string key in teremove)
            {
                files_with_same_hash.Remove(key);
            }

            ProgressWorker.I.EnQ("Adding to collection for previewing");
            DupChecker.DupplicationCollection.Clear();
            base.VMCleanerML.MaxProgress = files_with_same_hash.Count;
            base.VMCleanerML.ProgressIndex = 0;
            foreach (string entry in files_with_same_hash.Keys)
            {
                Model_DuplicateChecker e = new Model_DuplicateChecker();
                e.DuplicateFiles.AddRange(files_with_same_hash[entry].ToArray());
                e.Hash = entry;

                DupChecker.DupplicationCollection.Add(e);
            }

            // clear some memory
            files.Clear();
            files_with_same_size.Clear();
            files_to_hash.Clear();
            hashed_files.Clear();
            files_with_same_hash.Clear();

            foreach (Model_DuplicateChecker e in DupChecker.DupplicationCollection)
            {
                foreach (string file in e.DuplicateFiles)
                {
                    FileInfo fi = new FileInfo(file);
                    UpdateProgressLog(string.Format("Duplicate {2} \"{0}\" in \"{1}\" - {3}", fi.Name, fi.Directory.FullName, Win32API.FormatByteSize(fi.Length), e.Hash), "Retreiving duplicate files from the collection");
                }
            }
        }
        #endregion
    }
}