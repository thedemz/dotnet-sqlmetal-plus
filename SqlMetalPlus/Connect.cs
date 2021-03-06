using System;
using System.Linq;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using VSLangProj;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml.XPath;
using System.Diagnostics;
using stdole;
using System.Drawing;

namespace SqlMetalPlus
{
    /// <summary>The object for implementing an Add-in.</summary>
    /// <seealso class='IDTExtensibility2' />
    //[GuidAttribute("AC547FB9-7F74-4ec2-B541-8C0C557FCBA3"), ProgId("SqlMetalPlus.Connect")]
    public class Connect : IDTExtensibility2, IDTCommandTarget
    {
        private DTE2 _applicationObject;
        private AddIn _addInInstance;

        /// <summary>
        /// CommandBar command
        /// </summary>
        private CommandBar itemCmdBar;

        /// <summary>
        /// Project CommandBar command
        /// </summary>
        private CommandBar projectCmdBar;

        /// <summary>
        /// Folder CommandBar command
        /// </summary>
        private CommandBar folderCmdBar;

        /// <summary>
        /// Apply cstom Changes command
        /// </summary>
        private Command mergeCommand;

        /// <summary>
        /// Refresh DBML Command
        /// </summary>
        private Command refreshCommand;

        /// <summary>
        /// Create custom mapping xml Command
        /// </summary>
        private Command createMappingCommand;

        #region empty
        /// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
        public Connect()
        {
        }

        /// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />		
        public void OnAddInsUpdate(ref Array custom)
        {
        }

        /// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnStartupComplete(ref Array custom)
        {
        }

        /// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnBeginShutdown(ref Array custom)
        {
        }

        /// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
        /// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
        {
            if (disconnectMode == Extensibility.ext_DisconnectMode.ext_dm_HostShutdown ||
                disconnectMode == Extensibility.ext_DisconnectMode.ext_dm_UserClosed)
            {
                mergeCommand.Delete();
                refreshCommand.Delete();
                createMappingCommand.Delete();
            }
        }
        #endregion

        /// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
        /// <param term='application'>Root object of the host application.</param>
        /// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
        /// <param term='addInInst'>Object representing this Add-in.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
        {
            try
            {
                this._applicationObject = (DTE2)application;
                this._addInInstance = (AddIn)addInInst;

                // Only execute the startup code if the connection mode is a startup mode
                if (connectMode == ext_ConnectMode.ext_cm_Startup)
                {
                    var mergeCmdContextGUIDS = new object[] { };
                    var refreshCmdContextGUIDS = new object[] { };
                    var createMappingCmdContextGUIDS = new object[] { };


                    // Create a Command with name SolnExplContextMenuCS and then add it to the "Item" menubar for the SolutionExplorer
                    try
                    {
                        this.mergeCommand = this._applicationObject.Commands.Item("SqlMetalPlus.Connect.MergeCommand", -1);
                    }
                    catch
                    {
                        this.mergeCommand = this._applicationObject.Commands.AddNamedCommand(this._addInInstance,
                                                                                                 "MergeCommand",
                                                                                                 rm.GetString("MergeCaption", ci),
                                                                                                 rm.GetString("MergeCaptionTooltip", ci), true, 2189,
                                                                                                 ref mergeCmdContextGUIDS,
                                                                                                 (int)vsCommandStatus.vsCommandStatusSupported
                                                                                                 + (int)vsCommandStatus.vsCommandStatusEnabled);

                    }
                    //Check if it already exists,if not create it
                    try
                    {
                        this.refreshCommand = this._applicationObject.Commands.Item("SqlMetalPlus.Connect.RefreshCommand", -1);
                    }
                    catch
                    {
                        this.refreshCommand = this._applicationObject.Commands.AddNamedCommand(this._addInInstance,
                                                                                             "RefreshCommand",
                                                                                             rm.GetString("RefreshDBMLCaption", ci),
                                                                                             rm.GetString("RefreshDBMLTooltip", ci), true, 1977,
                                                                                             ref refreshCmdContextGUIDS,
                                                                                             (int)vsCommandStatus.vsCommandStatusSupported
                                                                                             + (int)vsCommandStatus.vsCommandStatusEnabled);
                    }
                    //Check if it already exists,if not create it
                    try
                    {
                        this.createMappingCommand = this._applicationObject.Commands.Item("SqlMetalPlus.Connect.CreateMappingCommand", -1);
                    }
                    catch
                    {
                        this.createMappingCommand = this._applicationObject.Commands.AddNamedCommand(this._addInInstance,
                                                                                                                         "CreateMappingCommand",
                                                                                                                         rm.GetString("CreateMappingCaption", ci),
                                                                                                                         rm.GetString("CreateMappingTooltip", ci), true, 2190,
                                                                                                                         ref createMappingCmdContextGUIDS,
                                                                                                                         (int)vsCommandStatus.vsCommandStatusSupported
                                                                                                                         + (int)vsCommandStatus.vsCommandStatusEnabled);
                    }

                    this.itemCmdBar = ((CommandBars)this._applicationObject.CommandBars)["Item"];
                    //this.projectCmdBar = ((CommandBars)this._applicationObject.CommandBars)["Project"];
                    //this.folderCmdBar = ((CommandBars)this._applicationObject.CommandBars)["Folder"];

                    if (this.itemCmdBar == null)
                    {
                        MessageBox.Show("Cannot get the Item menubar", "Error", MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                    }
                    else
                    {
                        if (!CommandExists(rm.GetString("MergeCaption", ci)))
                        {
                            this.itemCmdBar.Controls[1].BeginGroup = true; //start a group for the first command so that our commands appear in a seperate group at the top.
                            CommandBarControl ctrl = (CommandBarControl)this.refreshCommand.AddControl(this.itemCmdBar, 1);
                            //this will put a seperator above the command.you can take this aproach if you want your command to appear at the bottom or somewhere in the middle.
                            //ctrl.BeginGroup = true; 
                            this.mergeCommand.AddControl(this.itemCmdBar, 1);
                            this.createMappingCommand.AddControl(this.itemCmdBar, 1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(),"Unhandled Exception");
            }
        }
        private bool CommandExists(string commandId)
        {
            foreach (CommandBarControl ctrl in itemCmdBar.Controls)
            {
                if (ctrl.Caption == commandId)
                {
                    return true;
                }
            }
            return false;
        }
        #region IDTCommandTarget Members

        public void Exec(string CmdName, vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled)
        {
            try
            {
                Handled = false;
                if (ExecuteOption == vsCommandExecOption.vsCommandExecOptionDoDefault)
                {
                    UIHierarchy uIH = this._applicationObject.ToolWindows.SolutionExplorer;
                    var item = (UIHierarchyItem)((Array)uIH.SelectedItems).GetValue(0);

#pragma warning disable 168
                    UIHierarchyItems items = item.UIHierarchyItems;
#pragma warning restore 168

                    ProjectItem projectItem = uIH.DTE.SelectedItems.Item(1).ProjectItem;

                    Project proj = projectItem.ContainingProject;

                    string fileName = projectItem.get_FileNames(0);
                    if (!projectItem.Name.EndsWith(".dbml"))
                    {
                        Handled = true;
                        return;
                    }

                    if (CmdName == "SqlMetalPlus.Connect.MergeCommand")
                    {
                        ApplyManualChanges(projectItem, true);
                    }
                    if (CmdName == "SqlMetalPlus.Connect.RefreshCommand")
                    {
                        RefreshDBML(projectItem);
                    }
                    if (CmdName == "SqlMetalPlus.Connect.CreateMappingCommand")
                    {
                        CreateMappingXml(projectItem);
                    }
                    Handled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Unhandled Exception");
            }
        }



        public void QueryStatus(string CmdName, vsCommandStatusTextWanted NeededText, ref vsCommandStatus StatusOption, ref object CommandText)
        {
            if (NeededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
            {
                StatusOption = vsCommandStatus.vsCommandStatusInvisible;
                if (CmdName == "SqlMetalPlus.Connect.MergeCommand" ||
                    CmdName == "SqlMetalPlus.Connect.RefreshCommand")
                {
                    UIHierarchy uIH = this._applicationObject.ToolWindows.SolutionExplorer;
                    var item = (UIHierarchyItem)((Array)uIH.SelectedItems).GetValue(0);
                    if (item.Name.ToLower().EndsWith(".dbml"))
                    {
                        StatusOption = vsCommandStatus.vsCommandStatusSupported;
                        StatusOption |= vsCommandStatus.vsCommandStatusEnabled;
                    }
                }
                if (CmdName == "SqlMetalPlus.Connect.CreateMappingCommand")
                {
                    UIHierarchy uIH = this._applicationObject.ToolWindows.SolutionExplorer;
                    var item = (UIHierarchyItem)((Array)uIH.SelectedItems).GetValue(0);
                    if (item.Name.ToLower().EndsWith(".dbml") && !MappingExists(uIH.DTE.SelectedItems.Item(1).ProjectItem))
                    {
                        StatusOption = vsCommandStatus.vsCommandStatusSupported;
                        StatusOption |= vsCommandStatus.vsCommandStatusEnabled;
                    }
                }
                //TODO: or may be we can just add blank dbml using the VS way and use this tool
                //if (CmdName == "SqlMetalPlus.Connect.CreateNewCommand")
                //{
                //    UIHierarchy uIH = this._applicationObject.ToolWindows.SolutionExplorer;
                //    var item = (UIHierarchyItem)((Array)uIH.SelectedItems).GetValue(0);
                //    if (item is EnvDTE.Project || !item.Name.Contains(".")) //if project or a subfolder
                //    {
                //        StatusOption = vsCommandStatus.vsCommandStatusSupported;
                //        StatusOption |= vsCommandStatus.vsCommandStatusEnabled;
                //    }
                //}
            }

        }



        #endregion

        #region Command Implementation

        private const string ItemCommandBarName = "Item";
        private const string MergeCommandName = "MergeCommand";
        private const string RefreshCommandName = "RefreshCommand";
        protected const string ADDIN_NAME = "SqlMetalPlus";
        protected static XNamespace ns = "http://schemas.microsoft.com/linqtosql/dbml/2007";
        protected static ResourceManager rm = new ResourceManager("SqlMetalPlus.SqlMetalPlus", Assembly.GetExecutingAssembly());
        protected static CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentUICulture;

        /// <summary>
        /// Gets or sets the application.
        /// </summary>
        /// <value>The application.</value>
        public DTE2 Application
        {
            get { return _applicationObject; }
        }

        /// <summary>
        /// Gets the out put window called SqlMetalPlus for writing some messages.
        /// </summary>
        /// <value>The out put window.</value>
        public Window OutPutWindow
        {
            get
            {
                Windows w = this.Application.Windows;
                Window ww = w.Item(Constants.vsWindowKindOutput);
                return ww;
            }
        }
        /// <summary>
        /// Gets the out put window pane.
        /// </summary>
        /// <value>The out put window pane.</value>
        public OutputWindowPane OutPutWindowPane
        {
            get
            {
                OutputWindow o = (OutputWindow)this.OutPutWindow.Object;
                OutputWindowPanes oo = o.OutputWindowPanes;
                OutputWindowPane op = null;
                try
                {
                    op = oo.Item(ADDIN_NAME);
                }
                catch
                {
                    op = oo.Add(ADDIN_NAME);
                }
                return op;
            }
        }

        XElement doc;
        XElement mapping;
        string projectPath = string.Empty;
        const string CUSTOM_MAPPING_EXTN = ".custom.xml";
        EnvDTE.StatusBar statusBar = null;
        /// <summary>
        /// Applies the manual changes based on the custom mapping file(if found).
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="runCustomTool">if set to <c>true</c> [run custom tool].</param>
        public void ApplyManualChanges(ProjectItem item, bool runCustomTool)
        {
            statusBar = this.Application.StatusBar;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                this.OutPutWindowPane.Clear();
                this.OutPutWindowPane.Activate();
                //string path = Path.GetDirectoryName(item.ContainingProject.FullName);
                string dbmlPath = item.Properties.Item("FullPath").Value as string;
                CheckOut(item);
                string dbmlFolder = Path.GetDirectoryName(dbmlPath);
                //Get the custom mapping file.
                string mappingFile = item.Name + ".custom.xml";
                string mappingFilePath = Path.Combine(dbmlFolder, mappingFile);

                if (!File.Exists(mappingFilePath))
                {
                    this.OutPutWindowPane.OutputString("No Custom Mapping found.Please check whether the mapping file exists and with appropriate name:" + mappingFile + "\n");
                    return;
                }
                this.OutPutWindowPane.OutputString(string.Format("Merging manual changes from {0}{1}\n", mappingFile, Environment.NewLine));
                try
                {
                    doc = XElement.Load(dbmlPath);
                    mapping = XElement.Load(mappingFilePath);
                }
                catch (Exception)
                {
                    this.OutPutWindowPane.OutputString("Invalid or Empty dbml or custom mapping file.Please check whether the mapping file exists and with appropriate name:" + mappingFile + "\n");
                    return;
                }


                ApplyDatabaseChanges(doc);

                doc.Save(dbmlPath);
                if (runCustomTool)
                {
                    this.RunCustomTool(item);
                }
            }
            finally
            {
                if (runCustomTool)
                {
                    statusBar.Clear();
                    statusBar.Progress(false, string.Empty, 100, 100);
                    Cursor.Current = Cursors.Default;
                }
            }
        }
        /// <summary>
        /// Runs the custom tool on dbml to generate the code behind.
        /// </summary>
        /// <param name="item">The item.</param>
        private void RunCustomTool(ProjectItem item)
        {
            statusBar.Text = "Deleting the generated files...";
            this.OutPutWindowPane.OutputString("Deleting the generated files...\n");
            statusBar.Progress(true, string.Empty, 60, 100);
            //Delete existing Layout(.layout) and Designer.cs files
            foreach (ProjectItem child in item.ProjectItems)
            {
                child.Delete();
            }
            statusBar.Progress(true, string.Empty, 60, 100);


            //This is taking tooo long.
            VSProjectItem vsItem = item.Object as VSProjectItem;
            statusBar.Text = "Running the Custom Tool...";
            this.OutPutWindowPane.OutputString("Running the Custom Tool...\n");

            vsItem.RunCustomTool();
            statusBar.Text = "Custom Tool ran successfully";
            statusBar.Progress(true, string.Empty, 90, 100);
            //if you want to view errors generated ,
            //this.Application.DTE.Windows.Item(EnvDTE80.WindowKinds.vsWindowKindErrorList).Activate();
            //C:\Users\ak8853\Documents\Visual Studio 2008\Addins
            DialogResult dlgRes = DialogResult.No;
            dlgRes = MessageBox.Show("DBML generation successful.\nDo you want to open the file in designer?",
                                     "Success",
                                     MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dlgRes == DialogResult.Yes)
            {
                Window win = item.Open(EnvDTE.Constants.vsViewKindPrimary);
                win.SetFocus();
            }
            statusBar.Text = "Done";
        }
        /// <summary>
        /// Refreshes the DBML.
        /// </summary>
        /// <param name="item">The item.</param>
        public void RefreshDBML(ProjectItem item)
        {
            EnvDTE.StatusBar statusBar = this.Application.StatusBar;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                this.OutPutWindowPane.Clear();
                this.OutPutWindowPane.Activate();
                statusBar.Text = "Processing.Please wait...";
                statusBar.Animate(true, vsStatusAnimation.vsStatusAnimationGeneral);
                this.OutPutWindow.Visible = true;
                this.OutPutWindowPane.Activate();

                string dbmlPath = item.Properties.Item("FullPath").Value as string;
                string dbmlFolder = Path.GetDirectoryName(dbmlPath);
                string connString = string.Empty;
                try
                {
                    XElement config = XElement.Load(dbmlPath);
                    XElement connection = config.Element(ns + "Connection");
                    if (connection != null && connection.Attribute("ConnectionString") != null)
                    {
                        connString = (string)connection.Attribute("ConnectionString");
                    }
                }
                catch (Exception)
                {
                    //swallow the exception.
                }
                //connString = PromptForConnectionString(connString);
                ConnectionStringHelper helper = new ConnectionStringHelper();
                helper.Title = "SqlMetalPlus Connection Details";
                helper.IncludeSProcs = true;
                helper.Serialization = "Unidirectional";
                helper.ConnectionString = connString;
                if (helper.ShowDialog() == DialogResult.OK)
                {
                    connString = helper.ConnectionString;
                }
                else
                {
                    return;
                }
                CheckOut(item);
                this.OutPutWindow.Visible = true;
                this.OutPutWindowPane.Activate();
                //If the serialization is unidirectional then add reference to System.Runtime.Serialization
                if (helper.Serialization == "Unidirectional")
                {
                    AddReference(item.ContainingProject, "System.Runtime.Serialization.dll");
                }
                string command = @"C:\Program Files\Microsoft SDKs\Windows\v6.0A\bin\sqlmetal.exe";
                if (!File.Exists(command))
                {
                    //throw new Exception(@"SqlMetal.exe is not found at C:\Program Files\Microsoft SDKs\Windows\v6.0A\bin\");
                    command = @"sqlmetal.exe";
                }
                //string command = @"sqlmetal.exe";
                string arguments = string.Format("/conn:\"{0}\" /dbml:{1} /serialization:{2}{3}{4}{5}", connString, item.Name, helper.Serialization, helper.Pluralize ? " /pluralize" : "", helper.IncludeSProcs ? " /sprocs /functions" : "", helper.IncludeViews ? " /views" : "");
                //Directory.SetCurrentDirectory(dbmlFolder);
                System.Diagnostics.Process process = new System.Diagnostics.Process();

                ProcessStartInfo procInfo = new ProcessStartInfo();
                string sdkPath = @"C:\Program Files\Microsoft SDKs\Windows\v6.0A\bin\;";
                string currentPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);
                if (!currentPath.Contains(sdkPath))
                {
                    Environment.SetEnvironmentVariable("PATH", currentPath + (currentPath.EndsWith(";") ? "" : ";") + sdkPath, EnvironmentVariableTarget.Machine);
                    int result;
                    NativeMethods.SendMessageTimeout((System.IntPtr)NativeMethods.HWND_BROADCAST,
                    NativeMethods.WM_SETTINGCHANGE, 0, "Environment", NativeMethods.SMTO_BLOCK | NativeMethods.SMTO_ABORTIFHUNG |
                    NativeMethods.SMTO_NOTIMEOUTIFNOTHUNG, 5000, out result);

                }
                //string epath = procInfo.EnvironmentVariables["PATH"];
                //epath += (";" + @"C:\Program Files\Microsoft SDKs\Windows\v6.0A\bin\");
                //procInfo.EnvironmentVariables["Path"] = epath;
                procInfo.WorkingDirectory = dbmlFolder;
                procInfo.FileName = command;
                procInfo.Arguments = arguments;
                procInfo.UseShellExecute = false;
                procInfo.RedirectStandardError = true;
                procInfo.RedirectStandardInput = true;
                procInfo.RedirectStandardOutput = true;
                procInfo.CreateNoWindow = true;
                procInfo.WindowStyle = ProcessWindowStyle.Hidden;

                process.StartInfo = procInfo;

                this.OutPutWindowPane.OutputString(string.Format("{0} {1}{2}", command, arguments, Environment.NewLine));

                try
                {
                    process.Start();
                }
                catch (Exception)
                {
                    throw new Exception("the path for sqlmetal.exe not found.Correct the environment variable(PATH) and try again.");
                }

                StreamReader outputReader = null;
                StreamReader errorReader = null;
                //StreamWriter inputWriter = null;
                //inputWriter = process.StandardInput;
                outputReader = process.StandardOutput;
                errorReader = process.StandardError;
                process.WaitForExit();


                this.OutPutWindowPane.OutputString(outputReader.ReadToEnd());
                this.OutPutWindowPane.OutputString(errorReader.ReadToEnd());

                //Apply Manual Changes
                this.ApplyManualChanges(item, false);

                //Run the CustomTool
                this.RunCustomTool(item);
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("the path for sqlmetal.exe not found.Correct the environment variable(PATH) and try again.");
            }
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
            finally
            {
                statusBar.Clear();
                statusBar.Animate(false, null);
                statusBar.Progress(false, string.Empty, 100, 100);
                Cursor.Current = Cursors.Default;
            }
        }

        public void AddReference(Project project, string referencePath)
        {
            VSProject vsproject = project.Object as VSProject;
            Reference reference = null;
            try
            {
                reference = vsproject.References.Find(referencePath);
            }
            catch (Exception ex)
            {
                //Swallow it
            }
            if (reference == null)
            {
                vsproject.References.Add(referencePath);
            }
        }
        /// <summary>
        /// Checks out the dbml if integrated with any source control like TFS.
        /// </summary>
        /// <param name="item">The item.</param>
        private void CheckOut(ProjectItem item)
        {
            string dbmlPath = item.Properties.Item("FullPath").Value as string;
            FileInfo info = new FileInfo(dbmlPath);
            //foreach (Command cmd in item.DTE.Commands)
            //{
            //    Debug.Print(cmd.Name);
            //}
            if (info.IsReadOnly)
            {
                try
                {
                    //Command cmd = Application.Commands.Item("TfsCheckoutDynamicSilent", 11156);
                    //if (cmd.IsAvailable)
                    //{
                    //Try to checkout the file 
                    item.DTE.ExecuteCommand("TfsCheckoutDynamicSilent", "");
                    //}
                }
                catch
                {
                    throw new Exception("Unable to checkout the file automatically.\nCheckout the dbml manually and try again.");
                }
            }
        }
        private List<string> tablesToDelete = new List<string>();
        private List<string> functionsToDelete = new List<string>();
        private List<string> columnsToDelete = new List<string>();
        private List<string> associationsToDelete = new List<string>();
        /// <summary>
        /// Applies the database level changes like Namespace,classname etc.
        /// </summary>
        /// <param name="el">database element</param>
        private void ApplyDatabaseChanges(XElement el)
        {
            try
            {
                //string customizationType = (string)mapping.Attribute("CustomizationType");

                //if (customizationType == "Update")
                //{
                string newClassName = (string)mapping.Attribute("Class");
                if (!string.IsNullOrEmpty(newClassName))
                {
                    el.SetAttributeValue("Class", newClassName);
                }
                //Update Entity name Space
                string entityNamespace = (string)mapping.Attribute("EntityNamespace");
                if (!string.IsNullOrEmpty(entityNamespace))
                {
                    el.SetAttributeValue("EntityNamespace", entityNamespace);
                }
                //Update Context Name Space
                string contextNamespace = (string)mapping.Attribute("ContextNamespace");
                if (!string.IsNullOrEmpty(contextNamespace))
                {
                    el.SetAttributeValue("ContextNamespace", contextNamespace);
                }
                //}
                int noOfTables = doc.DescendantsAndSelf(ns + "Table").Count();
                int tableNo = 1;
                //Apply Table Changes
                tablesToDelete = new List<string>();
                foreach (var table in doc.DescendantsAndSelf(ns + "Table"))
                {
                    this.OutPutWindowPane.OutputString(string.Format("Processing {0}{1}", table.Attribute("Name").Value, Environment.NewLine));
                    statusBar.Text = string.Format("Processing {0}({1} of {2})", table.Attribute("Name").Value, tableNo++, noOfTables);
                    ApplyTableChanges(table);
                }
                foreach (string tableName in tablesToDelete)
                {
                    var table = (from a in doc.Elements(ns + "Table")
                                 where a.Attribute("Name").Value == tableName
                                 select a).FirstOrDefault();
                    if (table != null)
                    {
                        table.Remove();
                    }
                }
                //Apply Function Changes
                functionsToDelete = new List<string>();
                foreach (var functionElement in doc.DescendantsAndSelf(ns + "Function").ToList())
                {
                    ApplyFunctionChanges(functionElement);
                }
                foreach (string functionName in functionsToDelete)
                {
                    var functionNode = (from a in doc.Elements(ns + "Function")
                                        where a.Attribute("Name").Value == functionName
                                        select a).FirstOrDefault();
                    functionNode.Remove();
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// Applies the table level changes.
        /// </summary>
        /// <param name="el">Table Element</param>
        private void ApplyTableChanges(XElement el)
        {
            try
            {
                XElement objMappingTableNode;
                string tableName;

                tableName = (string)el.Attribute("Name");
                objMappingTableNode = (from a in mapping.Elements(ns + "Table")
                                       where a.Attribute("Name").Value == tableName
                                       select a).FirstOrDefault();
                if ((objMappingTableNode == null)) // No Mapping Necessary
                {
                    return;
                }
                string customizationType = (string)objMappingTableNode.Attribute("CustomizationType");
                if (string.IsNullOrEmpty(customizationType))
                {
                    customizationType = "Update";
                }
                if (customizationType == "Delete")
                {
                    this.OutPutWindowPane.OutputString(string.Format("Deleting Table:{0}{1}", tableName, Environment.NewLine));
                    tablesToDelete.Add(tableName);
                    //el.Remove();  //Remove this table from DBML
                    return;
                }
                if (customizationType == "Update")
                {
                    string newMemberName = (string)objMappingTableNode.Attribute("Member");
                    if (!string.IsNullOrEmpty(newMemberName))
                    {
                        el.SetAttributeValue("Member", newMemberName);
                    }

                    XElement typeNode = objMappingTableNode.Element(ns + "Type");
                    if (typeNode == null)
                    {
                        return; //no more column level customizations
                    }
                    //if Type is also modified, then update the same
                    string typeName = (string)typeNode.Attribute("Name");
                    if (!string.IsNullOrEmpty(typeName))
                    {
                        el.Element(ns + "Type").SetAttributeValue("Name", typeName);
                    }

                    string idName = (string)typeNode.Attribute("Id");
                    if (!string.IsNullOrEmpty(typeName))
                    {
                        el.Element(ns + "Type").SetAttributeValue("Id", idName);
                    }
                    columnsToDelete = new List<string>();
                    associationsToDelete = new List<string>();
                    foreach (var column in el.DescendantsAndSelf(ns + "Column"))
                    {
                        ApplyColumnChanges(column);
                    }
                    foreach (var column in columnsToDelete)
                    {
                        var columnNode = (from a in el.Element(ns + "Type").Elements(ns + "Column")
                                          where a.Attribute("Name").Value == column
                                          select a).FirstOrDefault();
                        columnNode.Remove();
                    }
                    //Add all the new columns added in the customisation
                    IEnumerable<XElement> newColumns = (from col in typeNode.Elements(ns + "Column")
                                                        where (string)col.Attribute("CustomizationType") == "Add"
                                                        select col).ToList();
                    foreach (var newColumn in newColumns)
                    {
                        //Check if its already there
                        if (!el.DescendantsAndSelf(ns + "Column").Any(c => (string)c.Attribute("Name") == (string)newColumn.Attribute("Name")))
                        {
                            var columnCustomizationType = newColumn.Attribute("CustomizationType");
                            columnCustomizationType.Remove(); //remove this attribute otherwise VS Designer will cry:)
                            //Add the new column to table
                            ((XElement)el.FirstNode).FirstNode.AddBeforeSelf(newColumn);
                        }
                    }
                    
                    foreach (var association in el.DescendantsAndSelf(ns + "Association"))
                    {
                        ApplyAssociationChanges(association);
                    }
                    foreach (var association in associationsToDelete)
                    {
                        var associationNode = (from a in el.Element(ns + "Type").Elements(ns + "Association")
                                               where a.Attribute("Name").Value == association
                                               select a).FirstOrDefault();
                        associationNode.Remove();
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                throw ex;
            }
        }
        /// <summary>
        /// Applies the column level changes.
        /// </summary>
        /// <param name="el">Column Element.</param>
        private void ApplyColumnChanges(XElement el)
        {
            try
            {
                XElement objMappingTableNode;
                XElement objMappingColumnNode;
                string tableName;
                string columnName;
                tableName = (string)el.Parent.Parent.Attribute("Name");
                columnName = (string)el.Attribute("Name");
                objMappingTableNode = (from a in mapping.Elements(ns + "Table")
                                       where a.Attribute("Name").Value == tableName
                                       select a).FirstOrDefault();
                if ((objMappingTableNode == null) ||
                    (objMappingTableNode.Element(ns + "Type") == null)) // No Mapping Necessary
                {
                    return;
                }

                objMappingColumnNode = (from a in objMappingTableNode.Element(ns + "Type").Elements(ns + "Column")
                                        where a.Attribute("Name").Value == columnName
                                        select a).FirstOrDefault();

                if (objMappingColumnNode == null) // No Mapping Necessary
                { return; }

                string customizationType = (string)objMappingColumnNode.Attribute("CustomizationType");
                if (string.IsNullOrEmpty(customizationType))
                {
                    customizationType = "Update";
                }
                if (customizationType == "Delete")
                {
                    columnsToDelete.Add(columnName);
                    //el.Remove(); //Remove this column
                    return;
                }
                if (customizationType == "Update")
                {
                    string newMemberName;
                    string newType;
                    if (objMappingColumnNode.Attribute("Member") != null)
                    {
                        newMemberName = objMappingColumnNode.Attribute("Member").Value;
                        el.SetAttributeValue("Member", newMemberName);
                    }
                    if (objMappingColumnNode.Attribute("Type") != null)
                    {
                        newType = objMappingColumnNode.Attribute("Type").Value;
                        el.SetAttributeValue("Type", newType);
                        //var tt = Type.GetType(newType.Replace("global::", ""));
                        //if (tt.BaseType.IsEnum) //then remove the association and parent lookup table
                        if (newType.Contains("global::"))
                        {
                            var association = (from a in el.Parent.Elements(ns + "Association")
                                                 where a.Attribute("ThisKey").Value == columnName
                                               select a).FirstOrDefault();
                            if (association != null)
                            {
                                associationsToDelete.Add(association.Attribute("Name").Value);
                                string fkTableType = association.Attribute("Type").Value;
                                var table = (from a in el.Parent.Parent.Parent.Elements(ns + "Table")
                                             where a.Element(ns + "Type").Attribute("Name").Value == fkTableType
                                             select a).FirstOrDefault();
                                if (table != null)
                                {
                                    tablesToDelete.Add(table.Attribute("Name").Value);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                throw ex;
            }
        }
        /// <summary>
        /// Applies the association(foreign keys) changes.
        /// </summary>
        /// <param name="el">Association element</param>
        private void ApplyAssociationChanges(XElement el)
        {
            try
            {
                XElement objMappingTableNode;
                XElement objMappingAssociationNode;
                string tableName, associationName;
                string isForeignKey = string.Empty;

                tableName = el.Parent.Parent.Attribute("Name").Value;
                associationName = el.Attribute("Name").Value;
                if (el.Attribute("IsForeignKey") != null)
                {
                    isForeignKey = el.Attribute("IsForeignKey").Value;
                }
                objMappingTableNode = (from a in mapping.Elements(ns + "Table")
                                       where a.Attribute("Name").Value == tableName
                                       select a).FirstOrDefault();
                if ((objMappingTableNode == null) ||
                    (objMappingTableNode.Element(ns + "Type") == null)) // No Mapping Necessary
                {
                    return;
                }

                if (isForeignKey != "")
                {
                    objMappingAssociationNode = (from a in objMappingTableNode.Element(ns + "Type").Elements(ns + "Association")
                                                 where a.Attribute("Name").Value == associationName &&
                                                       (a.Attribute("IsForeignKey") != null &&
                                                       a.Attribute("IsForeignKey").Value == isForeignKey)
                                                 select a).FirstOrDefault();
                }
                else
                {
                    objMappingAssociationNode = (from a in objMappingTableNode.Element(ns + "Type").Elements(ns + "Association")
                                                 where a.Attribute("Name").Value == associationName &&
                                                 a.Attribute("IsForeignKey") == null
                                                 select a).FirstOrDefault();
                }


                if (objMappingAssociationNode == null) // No Mapping Necessary
                {
                    return;
                }
                string customizationType = (string)objMappingAssociationNode.Attribute("CustomizationType");
                if (string.IsNullOrEmpty(customizationType))
                {
                    customizationType = "Update";
                }
                if (customizationType == "Delete")
                {
                    // el.Remove(); //Remove this Association from DBML
                    associationsToDelete.Add(associationName);
                    return;
                }
                if (customizationType == "Update") // Only Update is Allowed for Associations
                {
                    string newMemberName, cardinality;
                    if (objMappingAssociationNode.Attribute("Member") != null)
                    {
                        newMemberName = objMappingAssociationNode.Attribute("Member").Value;
                        el.SetAttributeValue("Member", newMemberName);
                    }
                    if (objMappingAssociationNode.Attribute("Cardinality") != null)
                    {
                        cardinality = objMappingAssociationNode.Attribute("Cardinality").Value;
                        el.SetAttributeValue("Cardinality", cardinality);
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                throw ex;
            }
        }
        /// <summary>
        /// Applies the function/stored procedure changes.
        /// </summary>
        /// <param name="el">Function Element</param>
        private void ApplyFunctionChanges(XElement el)
        {
            try
            {
                XElement objMappingFunctionNode;
                string functionName;
                functionName = el.Attribute("Name").Value;

                objMappingFunctionNode = (from a in mapping.Elements(ns + "Function")
                                          where a.Attribute("Name").Value == functionName
                                          select a).FirstOrDefault();
                if (objMappingFunctionNode == null) // No Mapping Necessary
                {
                    return;
                }
                string customizationType = (string)objMappingFunctionNode.Attribute("CustomizationType");
                if (string.IsNullOrEmpty(customizationType))
                {
                    customizationType = "Update";
                }
                if (customizationType == "Delete")
                {
                    // el.Remove(); //Remove this function from DBML
                    functionsToDelete.Add(functionName);
                    return;
                }
                if (customizationType == "Update")
                {
                    string newMemberName;
                    if (objMappingFunctionNode.Attribute("Method") != null)
                    {
                        newMemberName = objMappingFunctionNode.Attribute("Method").Value;
                        el.SetAttributeValue("Method", newMemberName);
                    }
                }
                foreach (var parameter in el.DescendantsAndSelf(ns + "Parameter"))
                {
                    ApplyFunctionParameterChanges(parameter);
                }
                foreach (var elementType in el.DescendantsAndSelf(ns + "ElementType"))
                {
                    ApplyFunctionElementTypeChanges(elementType);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                throw ex;
            }
        }
        private void ApplyFunctionParameterChanges(XElement el)
        {
            try
            {
                XElement objMappingFunctionNode;
                XElement objMappingParameterNode;
                string functionName;
                string parameterName;
                functionName = (string)el.Parent.Attribute("Name");
                parameterName = (string)el.Attribute("Name");
                objMappingFunctionNode = (from a in mapping.Elements(ns + "Function")
                                          where a.Attribute("Name").Value == functionName
                                          select a).FirstOrDefault();
                objMappingParameterNode = (from a in objMappingFunctionNode.Elements(ns + "Parameter")
                                           where a.Attribute("Name").Value == parameterName
                                           select a).FirstOrDefault();

                if (objMappingParameterNode == null) // No Mapping Necessary
                { return; }

                string customizationType = (string)objMappingParameterNode.Attribute("CustomizationType");
                if (string.IsNullOrEmpty(customizationType))
                {
                    customizationType = "Update";
                }
                if (customizationType == "Update")
                {
                    string newName;
                    if (objMappingParameterNode.Attribute("Parameter") != null)
                    {
                        newName = objMappingParameterNode.Attribute("Parameter").Value;
                        el.SetAttributeValue("Parameter", newName);
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                throw ex;
            }
        }
        private void ApplyFunctionElementTypeChanges(XElement el)
        {
            try
            {
                XElement objMappingFunctionNode;
                XElement objMappingElementTypeNode;
                string functionName;
                string elementTypeName;
                functionName = el.Parent.Attribute("Name").Value;
                elementTypeName = el.Attribute("Name").Value;

                objMappingFunctionNode = (from a in mapping.Elements(ns + "Function")
                                          where a.Attribute("Name").Value == functionName
                                          select a).FirstOrDefault();
                if (objMappingFunctionNode == null) // No Mapping Necessary
                {
                    return;
                }
                objMappingElementTypeNode = (from a in objMappingFunctionNode.Elements(ns + "ElementType")
                                             select a).FirstOrDefault();
                if (objMappingElementTypeNode == null) // No Mapping Necessary
                {
                    return;
                }

                string customizationType = (string)objMappingElementTypeNode.Attribute("CustomizationType");
                if (string.IsNullOrEmpty(customizationType))
                {
                    customizationType = "Update";
                }
                if (customizationType == "Update")
                {
                    string newName;
                    if (objMappingElementTypeNode.Attribute("Name") != null)
                    {
                        newName = objMappingElementTypeNode.Attribute("Name").Value;
                        el.SetAttributeValue("Name", newName);
                    }
                }
                foreach (var column in el.DescendantsAndSelf(ns + "Column"))
                {
                    ApplyFunctionElementTypeColumnChanges(column);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                throw ex;
            }
        }
        private void ApplyFunctionElementTypeColumnChanges(XElement el)
        {
            try
            {
                XElement objMappingFunctionNode;
                XElement objMappingElementTypeNode;
                XElement objMappingElementTypeColumnNode;
                string functionName;
                string elementTypeName;
                string elementTypeColumnName;
                functionName = (string)el.Parent.Parent.Attribute("Name");
                elementTypeName = el.Parent.Attribute("Name").Value;
                elementTypeColumnName = (string)el.Attribute("Name");
                objMappingFunctionNode = (from a in mapping.Elements(ns + "Function")
                                          where a.Attribute("Name").Value == functionName
                                          select a).FirstOrDefault();
                if (objMappingFunctionNode == null) // No Mapping Necessary
                { return; }
                objMappingElementTypeNode = (from a in objMappingFunctionNode.Elements(ns + "ElementType")
                                             select a).FirstOrDefault();
                if (objMappingElementTypeNode == null) // No Mapping Necessary
                { return; }
                objMappingElementTypeColumnNode = (from a in objMappingElementTypeNode.Elements(ns + "Column")
                                                   where a.Attribute("Name").Value == elementTypeColumnName
                                                   select a).FirstOrDefault();

                if (objMappingElementTypeColumnNode == null) // No Mapping Necessary
                { return; }

                string customizationType = (string)objMappingElementTypeColumnNode.Attribute("CustomizationType");
                if (string.IsNullOrEmpty(customizationType))
                {
                    customizationType = "Update";
                }
                if (customizationType == "Update")
                {
                    string newMemberName;
                    string newType;
                    if (objMappingElementTypeColumnNode.Attribute("Member") != null)
                    {
                        newMemberName = objMappingElementTypeColumnNode.Attribute("Member").Value;
                        el.SetAttributeValue("Member", newMemberName);
                    }
                    if (objMappingElementTypeColumnNode.Attribute("Type") != null)
                    {
                        newType = objMappingElementTypeColumnNode.Attribute("Type").Value;
                        el.SetAttributeValue("Type", newType);
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                throw ex;
            }
        }
        /// <summary>
        /// Creates the mapping XML.
        /// </summary>
        /// <param name="projectItem">The project item.</param>
        private void CreateMappingXml(ProjectItem projectItem)
        {
            string dbmlPath = projectItem.Properties.Item("FullPath").Value as string;
            File.Copy(dbmlPath, dbmlPath + CUSTOM_MAPPING_EXTN, true);
            var parent = projectItem.Collection.Parent;
            if (parent is ProjectItem)
            {
                (parent as ProjectItem).ProjectItems.AddFromFileCopy(dbmlPath + CUSTOM_MAPPING_EXTN);
            }
            if (parent is Project)
            {
                (parent as Project).ProjectItems.AddFromFileCopy(dbmlPath + CUSTOM_MAPPING_EXTN);
            }

        }
        /// <summary>
        /// Check if a mapping file already exists.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><see cref="Boolean"/></returns>
        private bool MappingExists(ProjectItem item)
        {
            var parent = item.Collection.Parent;
            if (parent is ProjectItem)
            {
                foreach (ProjectItem child in (parent as ProjectItem).ProjectItems)
                {
                    if (child.Name == item.Name + CUSTOM_MAPPING_EXTN)
                    {
                        return true;
                    }
                }
            }
            if (parent is Project)
            {
                foreach (ProjectItem child in (parent as Project).ProjectItems)
                {
                    if (child.Name == item.Name + CUSTOM_MAPPING_EXTN)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion

        #region Icon Members
        private stdole.IPictureDisp getImage()
        {
            stdole.IPictureDisp tempImage = null;
            try
            {
                System.Drawing.Icon newIcon = SqlMetalPlus.Web_XML;

                ImageList newImageList = new ImageList();
                newImageList.Images.Add(newIcon);
                tempImage = ConvertImage.Convert(newImageList.Images[0]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return tempImage;
        }
        #endregion
    }
    sealed public class ConvertImage : System.Windows.Forms.AxHost
    {
        private ConvertImage()
            : base(null)
        {
        }
        public static IPictureDisp Convert(Image image)
        {
            return (IPictureDisp)AxHost.GetIPictureDispFromPicture(image);
        }
    }




}
