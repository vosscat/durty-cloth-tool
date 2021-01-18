﻿using Microsoft.Win32;
using System.IO;
using System.Linq;

namespace AltTool
{
    class ProjectController
    {
        private static ProjectController _singleton;
        public static ProjectController Instance()
        {
            return _singleton ?? (_singleton = new ProjectController());
        }

        public void AddFiles(ClothData.Sex targetSex)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                CheckFileExists = true,
                Filter = "Clothes geometry (*.ydd)|*.ydd",
                FilterIndex = 1,
                DefaultExt = "ydd",
                Multiselect = true,
                Title = "Adding " + (targetSex == ClothData.Sex.Male ? "male" : "female") + " clothes"
            };

            if (openFileDialog.ShowDialog() != true) 
                return;

            foreach (string filename in openFileDialog.FileNames)
            {
                string baseFileName = Path.GetFileName(filename);
                ClothNameResolver cData = new ClothNameResolver(baseFileName);

                if(!cData.IsVariation)
                {
                    ClothData nextCloth = new ClothData(filename, cData.ClothType, cData.DrawableType, cData.BindedNumber, cData.Postfix, targetSex);
                        
                    if(cData.ClothType == ClothNameResolver.ClothTypes.Component)
                    {
                        nextCloth.SearchForFirstPersonModel();
                        nextCloth.SearchForTextures();

                        var clothes = MainWindow.Clothes.ToList();
                        clothes.Add(nextCloth);
                        clothes = clothes.OrderBy(x => x.Name, new AlphanumericComparer()).ToList();
                        MainWindow.Clothes.Clear();

                        foreach(var cloth in clothes)
                        {
                            MainWindow.Clothes.Add(cloth);
                        }

                        StatusController.SetStatus(nextCloth + " added (" + (!string.IsNullOrEmpty(nextCloth.FirstPersonModelPath)? "FP Model found, " : "") + "Found " + nextCloth.Textures.Count + " textures). Total clothes: " + MainWindow.Clothes.Count);
                    }
                    else
                    {
                        nextCloth.SearchForTextures();

                        var clothes = MainWindow.Clothes.ToList();
                        clothes.Add(nextCloth);
                        clothes = clothes.OrderBy(x => x.Name, new AlphanumericComparer()).ToList();
                        MainWindow.Clothes.Clear();

                        foreach (var cloth in clothes)
                        {
                            MainWindow.Clothes.Add(cloth);
                        }

                        StatusController.SetStatus(nextCloth + " added. (Found " + nextCloth.Textures.Count + " textures). Total clothes: " + MainWindow.Clothes.Count);
                    }
                }
                else
                    StatusController.SetStatus("Item " + baseFileName + " can't be added. Looks like it's variant of another item");
            }
        }

        public void AddTexture(ClothData cloth)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                CheckFileExists = true,
                Filter = "Clothes texture (*.ytd)|*.ytd",
                FilterIndex = 1,
                DefaultExt = "ytd",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() != true) 
                return;

            foreach (string filename in openFileDialog.FileNames.Where(f => f.EndsWith(".ytd")))
            {
                cloth.AddTexture(filename);
            }
        }

        public void SetFirstPersonModel(ClothData cloth)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                CheckFileExists = true,
                Filter = "Clothes drawable (*.ydd)|*.ydd",
                FilterIndex = 1,
                DefaultExt = "ydd",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() != true) 
                return;

            foreach (string filename in openFileDialog.FileNames.Where(f => f.EndsWith(".ydd")))
            {
                cloth.SetFirstPersonModel(filename);
            }
        }
    }
}
