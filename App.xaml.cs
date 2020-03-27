using System;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows;


namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {




        public App()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            string RequiredDllName = $"{(new AssemblyName(args.Name)).Name}.dll";
            string resource = currentAssembly.GetManifestResourceNames().Where(s => s.EndsWith(RequiredDllName)).FirstOrDefault();

            if (resource != null)
            {
                using (System.IO.Stream stream = currentAssembly.GetManifestResourceStream(resource))
                {
                    if (stream == null)
                    {
                        return null;
                    }

                    byte[] block = new byte[stream.Length];
                    stream.Read(block, 0, block.Length);
                    return Assembly.Load(block);
                }
            }
            else
            {
                return null;
            }
        }
    }
}
