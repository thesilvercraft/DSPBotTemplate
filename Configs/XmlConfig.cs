using DSharpPlus.Entities;
using DSPBotTemplate.Attributes;
using DSPBotTemplate.Utils;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace DSPBotTemplate.Configs
{
    internal class XmlConfig : IConfig
    {
        private XmlConfigFormat xmlConfig;
        private const ulong CurrentConfigVer = 0;
        private static string filename;

        private static string FileName()
        {
            if (string.IsNullOrEmpty(filename))
            {
                filename = Assembly.GetExecutingAssembly().GetName().Name + ".xml";
            }
            return filename;
        }

        private static XmlDocument MakeDocumentWithComments(XmlDocument xmlDocument)
        {
            foreach (var i in typeof(XmlConfigFormat).GetMembers())
            {
                foreach (var e in i.GetCustomAttributes(false))
                {
                    if (e.GetType() == typeof(XmlDescriptionAttribute))
                    {
                        xmlDocument = XmlUtils.CommentBeforeObject(xmlDocument, $"/XmlConfigFormat/{i.Name}", ((XmlDescriptionAttribute)e).description);
                    }
                }
            }

            return xmlDocument;
        }

        public static async Task OutdatedConfigTask(XmlConfigFormat readconfig, Logger logger)
        {
            logger.Fatal("Outdated config detected. Would you like a new one to generate? (Y/n)");
            var rl = Console.ReadLine();
            if (rl != null)
                switch (rl.ToLower())
                {
                    case "y":
                        using (var streamReader = new StreamReader(FileName()))
                        {
                            await using (var streamWriter = new StreamWriter(FileName(), false))
                            {
                                await streamWriter.WriteAsync(await streamReader.ReadToEndAsync());
                                streamWriter.Close();
                            }

                            streamReader.Close();
                        }

                        await using (var streamWriter = new StreamWriter(FileName()))
                        {
                            readconfig.ConfigVer = CurrentConfigVer;
                            MakeDocumentWithComments(XmlUtils.SerializeToXmlDocument(readconfig)).Save(streamWriter);
                            streamWriter.Close();
                        }

                        new Process
                        {
                            StartInfo = new ProcessStartInfo(Environment.CurrentDirectory + "\\" + FileName())
                            {
                                UseShellExecute = true
                            }
                        }.Start();
                        new Process
                        {
                            StartInfo = new ProcessStartInfo(Environment.CurrentDirectory + "\\DSPBotTemplateOld.xml")
                            {
                                UseShellExecute = true
                            }
                        }.Start();
                        logger.Fatal(
                            $"Ok now transfer the values from silverbotold.xml to {FileName()} and restart thx");
                        Environment.Exit(420);
                        break;

                    case "n":
                        logger.Fatal(
                             "Outdated config detected. Would you like to try loading with the outdated config? (Y/n)");
                        var nrl = Console.ReadLine();
                        if (nrl != null)
                            switch (nrl.ToLower())
                            {
                                case "y":
                                    break;

                                case "n":
                                    Environment.Exit(421);
                                    break;

                                default:
                                    await OutdatedConfigTask(readconfig, logger);
                                    break;
                            }

                        break;

                    default:
                        await OutdatedConfigTask(readconfig, logger);
                        break;
                }
        }

        public async Task<bool> StartUp(Logger logger)
        {
            var serializer = new XmlSerializer(typeof(XmlConfigFormat));

            if (!File.Exists(FileName()))
            {
                await using (var streamWriter = new StreamWriter(FileName()))
                {
                    MakeDocumentWithComments(XmlUtils.SerializeToXmlDocument(new XmlConfigFormat
                    {
                        ConfigVer = CurrentConfigVer
                    })).Save(streamWriter);
                    streamWriter.Close();
                }
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    new Process
                    {
                        StartInfo = new ProcessStartInfo(Environment.CurrentDirectory + "\\" + FileName())
                        {
                            UseShellExecute = true
                        }
                    }.Start();
                    logger.Fatal($"{FileName()} should have opened in the default app, edit it, save it and press enter");
                    logger.Fatal("Press any key WHEN READY to continue...");
                    Console.ReadKey();
                }
                else
                {
                    logger.Fatal($"{FileName()} should exist in the Current Working Directory, edit it, save it and restart silverbot");
                    logger.Fatal("Press any key to continue...");
                    Console.ReadKey();
                    Environment.Exit(420);
                }
            }

            await using var fs = File.Open(FileName(), FileMode.Open);
            var readconfig = serializer.Deserialize(fs) as XmlConfigFormat;
            fs.Close();
            if (readconfig != null && (readconfig.ConfigVer == null || readconfig.ConfigVer != CurrentConfigVer))
            {
                await OutdatedConfigTask(readconfig, logger);
            }
            else
            {
                xmlConfig = readconfig;
            }

            return true;
        }

        public Task<Tuple<DiscordActivity, TimeSpan?>> Activity()
        {
            using RandomGenerator rg = new();
            return Task.FromResult(new Tuple<DiscordActivity, TimeSpan?>(xmlConfig.Activities[rg.Next(0, xmlConfig.Activities.Length)].ToDiscordActivity(), TimeSpan.FromMilliseconds((double)xmlConfig.MsInterval)));
        }

        public Task<string[]> Prefixes()
        {
            return Task.FromResult(xmlConfig.Prefixes ?? throw new NullReferenceException());
        }

        public Task<string> DiscordToken()
        {
            return Task.FromResult(xmlConfig.Token);
        }

        //TODO: IMPLEMENT MORE METHODS HERE
    }
}