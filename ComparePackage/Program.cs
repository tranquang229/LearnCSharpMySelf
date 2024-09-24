using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace ComparePackage
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var packages = new List<PackageDto>();

            var directories = new List<(string Path, bool IsNetFramework)>()
            {
                (@"D:\Projects\D2C\APACResponsiveDefault\ElectroluxAPAC", true),
                (@"D:\Projects\D2C\Niteco.D2C.NetCore.Accelerator\ElectroluxAPAC", false),
            };


            foreach (var directory in directories)
            {
                var packagesInPackagesConfigureFiles = ScanPackagesInPackagesConfigureFiles(directory.Path, directory.IsNetFramework);
                var packagesInCsProjectFiles = ScanPackagesInCsProjectFiles(directory.Path, directory.IsNetFramework);
               
                packages.AddRange(packagesInPackagesConfigureFiles);
                packages.AddRange(packagesInCsProjectFiles);
            }

            var ignoredPackages = new List<string>
            {
                //"System.",
                //"Microsoft.",
                //"EPiServer.",
            };
            var isOnlyListOutMissingPackage = false;
            var isShowProjectName = true;

            GenerateCsvFile(packages, ignoredPackages, isOnlyListOutMissingPackage, isShowProjectName);

            //var packageGroups = packages.GroupBy(x => new { x.Name, x.Version })
            //    .Select(g => new
            //    {
            //        g.Key.Name,
            //        g.Key.Version,
            //        Projects = string.Join(", ", g.Select(x => x.Project)),
            //        Url = $"https://www.nuget.org/packages/{g.Key.Name}/{g.Key.Version}"
            //    })
            //    .OrderBy(x => x.Name)
            //    .ThenBy(x => x.Version).ToList();

            //var ignoredPackages = new List<string>
            //{
            //    //"System.",
            //    //"Microsoft."
            //};

            //using (var fileStream = File.Open("packages.csv", FileMode.Create))
            //{
            //    using (var streamWriter = new StreamWriter(fileStream))
            //    {
            //        foreach (var package in packageGroups)
            //        {
            //            if (ignoredPackages.Any(x => package.Name.StartsWith(x)))
            //            {
            //                continue;
            //            }

            //            streamWriter.WriteLine($"{package.Name},{package.Version}, ,\"{package.Url}\",\"{package.Projects}\"");
            //        }
            //    }
            //}


        }

        private static void GenerateCsvFile(List<PackageDto> packages, List<string> ignoredPackages, 
            bool isOnlyListOutMissingPackage, bool isShowProjectName)
        {
            var listPackageNamePrinted = new List<string>();

            using (var fileStream = File.Open("packages.csv", FileMode.Create))
            {
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    var packagesNetFramework = packages.Where(x => x.IsNetFramework).ToList();
                    var packagesNetCore = packages.Where(x => !x.IsNetFramework).ToList();

                    var grpPackagesNetFramework = packagesNetFramework.GroupBy(x => x.Project);
                    foreach (var grpNetFramework in grpPackagesNetFramework)
                    {
                        //streamWriter.WriteLine($"------- {grpNetFramework.Key} -------");
                        foreach (var package in grpNetFramework.ToList())
                        {
                            if (ignoredPackages.Any(x => package.Name.StartsWith(x)))
                            {
                                continue;
                            }

                            var packageInNetCoreRelative = packagesNetCore
                                .FirstOrDefault(x => x.Project == grpNetFramework.Key && x.Name == package.Name);

                            if (isShowProjectName)
                            {
                                if (isOnlyListOutMissingPackage)
                                {
                                    if (packageInNetCoreRelative == null)
                                    {
                                        streamWriter.WriteLine($"{package.Project}|{package.Name}|{package.Version}");
                                    }
                                }
                                else
                                {
                                    streamWriter.WriteLine(packageInNetCoreRelative != null
                                        ? $"{package.Project}|{package.Name}|{package.Version}|{packageInNetCoreRelative.Name}|{packageInNetCoreRelative.Version}"
                                        : $"{package.Project}|{package.Name}|{package.Version}");
                                }
                            }
                            else
                            {
                                if (!listPackageNamePrinted.Contains(package.Name))
                                {
                                    if (isOnlyListOutMissingPackage)
                                    {
                                        if (packageInNetCoreRelative == null)
                                        {
                                            streamWriter.WriteLine($"{package.Name}|{package.Version}");
                                        }
                                    }
                                    else
                                    {
                                        streamWriter.WriteLine(packageInNetCoreRelative != null
                                            ? $"{package.Name}|{package.Version}|{packageInNetCoreRelative.Name}|{packageInNetCoreRelative.Version}"
                                            : $"{package.Name}|{package.Version}");
                                    }
                                }
                               
                                listPackageNamePrinted.Add(package.Name);
                            }
                        }
                    }
                }
            }
        }

        static List<PackageDto> ScanPackagesInPackagesConfigureFiles(string directory, bool isNetFramework)
        {
            var files = Directory.GetFiles(directory, "packages.config", SearchOption.AllDirectories);
            var packages = new List<PackageDto>();
            foreach (var file in files)
            {
                var projectName = new DirectoryInfo(Path.GetDirectoryName(file)).Name;
                XDocument xdoc = XDocument.Load(file);
                var packagesNode = xdoc.Descendants("packages").First();
                var packageNodes = packagesNode.Descendants("package");
                foreach (var node in packageNodes)
                {
                    var packageName = node.Attribute("id")?.Value;
                    var packageVersion = node.Attribute("version")?.Value;

                    packages.Add(new PackageDto()
                    {
                        Name = packageName,
                        Version = packageVersion,
                        Project = projectName,
                        IsNetFramework = isNetFramework
                    } );
                }
            }

            return packages;
        }

        static List<PackageDto> ScanPackagesInCsProjectFiles(string directory, bool isNetFramework)
        {
            var files = Directory.GetFiles(directory, "*.csproj", SearchOption.AllDirectories);
            var packages = new List<PackageDto>();
            foreach (var file in files)
            {
                var projectName = new DirectoryInfo(Path.GetDirectoryName(file)).Name;
                XDocument xdoc = XDocument.Load(file);
                var ItemGroupNodes = xdoc.Descendants("ItemGroup");
                foreach (var ItemGroupNode in ItemGroupNodes)
                {
                    var packageNodes = ItemGroupNode.Descendants("PackageReference");
                    foreach (var node in packageNodes)
                    {
                        var packageName = node.Attribute("Include")?.Value;
                        var packageVersion = node.Attribute("Version")?.Value;

                        if (string.IsNullOrWhiteSpace(packageName))
                            continue;

                        packages.Add(new PackageDto()
                        {
                            Name = packageName,
                            Version = packageVersion,
                            Project = projectName,
                            IsNetFramework = isNetFramework
                        });
                    }
                }

            }

            if (packages.Count > 0)
            {
                var test = true;
            }
           
            return packages;
        }
    }
}
