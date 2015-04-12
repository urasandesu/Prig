/* 
 * File: PackageViewModel.cs
 * 
 * Author: Akira Sugiura (urasandesu@gmail.com)
 * 
 * 
 * Copyright (c) 2015 Akira Sugiura
 *  
 *  This software is MIT License.
 *  
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *  
 *  The above copyright notice and this permission notice shall be included in
 *  all copies or substantial portions of the Software.
 *  
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 *  THE SOFTWARE.
 */



using EnvDTE;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using Urasandesu.Prig.VSPackage.Infrastructure;

namespace Urasandesu.Prig.VSPackage
{
    class PrigPackageViewModel : PackageViewModel
    {
        PackageProperty<bool> m_isTestAdapterEnabled;
        public PackageProperty<bool> IsTestAdapterEnabled
        {
            get
            {
                if (m_isTestAdapterEnabled == null)
                    m_isTestAdapterEnabled = new PackageProperty<bool>();
                return m_isTestAdapterEnabled;
            }
        }

        PackageProperty<Project> m_currentProject;
        public PackageProperty<Project> CurrentProject
        {
            get
            {
                if (m_currentProject == null)
                    m_currentProject = new PackageProperty<Project>();
                return m_currentProject;
            }
        }

        PackageProperty<bool> m_isEditPrigIndirectionSettingsCommandVisible;
        public PackageProperty<bool> IsEditPrigIndirectionSettingsCommandVisible
        {
            get
            {
                if (m_isEditPrigIndirectionSettingsCommandVisible == null)
                    m_isEditPrigIndirectionSettingsCommandVisible = new PackageProperty<bool>(true);
                return m_isEditPrigIndirectionSettingsCommandVisible;
            }
        }

        AddPrigAssemblyForMSCorLibCommand m_addPrigAssemblyForMSCorLibCommand;
        public ICommand AddPrigAssemblyForMSCorLibCommand
        {
            get
            {
                if (m_addPrigAssemblyForMSCorLibCommand == null)
                    m_addPrigAssemblyForMSCorLibCommand = BuildUpPackageCommand(new AddPrigAssemblyForMSCorLibCommand(this));
                return m_addPrigAssemblyForMSCorLibCommand;
            }
        }

        AddPrigAssemblyCommand m_addPrigAssemblyCommand;
        public ICommand AddPrigAssemblyCommand
        {
            get
            {
                if (m_addPrigAssemblyCommand == null)
                    m_addPrigAssemblyCommand = BuildUpPackageCommand(new AddPrigAssemblyCommand(this));
                return m_addPrigAssemblyCommand;
            }
        }

        EnableTestAdapterCommand m_enableTestAdapterCommand;
        public ICommand EnableTestAdapterCommand
        {
            get
            {
                if (m_enableTestAdapterCommand == null)
                {
                    var canExecuteSource =
                            IsTestAdapterEnabled.Select(_ => Unit.Default).
                            Merge(CurrentProject.Select(_ => Unit.Default)).
                            Select(_ => !IsTestAdapterEnabled.Value && CurrentProject.Value != null);
                    m_enableTestAdapterCommand = BuildUpPackageCommand(new EnableTestAdapterCommand(this, canExecuteSource));
                }
                return m_enableTestAdapterCommand;
            }
        }

        TestAdapterBeforeQueryStatusCommand m_testAdapterBeforeQueryStatusCommand;
        public ICommand TestAdapterBeforeQueryStatusCommand
        {
            get
            {
                if (m_testAdapterBeforeQueryStatusCommand == null)
                    m_testAdapterBeforeQueryStatusCommand = BuildUpPackageCommand(new TestAdapterBeforeQueryStatusCommand(this));
                return m_testAdapterBeforeQueryStatusCommand;
            }
        }

        DisableTestAdapterCommand m_disableTestAdapterCommand;
        public ICommand DisableTestAdapterCommand
        {
            get
            {
                if (m_disableTestAdapterCommand == null)
                    m_disableTestAdapterCommand = BuildUpPackageCommand(new DisableTestAdapterCommand(this, IsTestAdapterEnabled));
                return m_disableTestAdapterCommand;
            }
        }

        EditPrigIndirectionSettingsCommand m_editPrigIndirectionSettingsCommand;
        public ICommand EditPrigIndirectionSettingsCommand
        {
            get
            {
                if (m_editPrigIndirectionSettingsCommand == null)
                    m_editPrigIndirectionSettingsCommand = BuildUpPackageCommand(new EditPrigIndirectionSettingsCommand(this));
                return m_editPrigIndirectionSettingsCommand;
            }
        }

        RemovePrigAssemblyCommand m_removePrigAssemblyCommand;
        public ICommand RemovePrigAssemblyCommand
        {
            get
            {
                if (m_removePrigAssemblyCommand == null)
                    m_removePrigAssemblyCommand = BuildUpPackageCommand(new RemovePrigAssemblyCommand(this));
                return m_removePrigAssemblyCommand;
            }
        }

        EditPrigIndirectionSettingsBeforeQueryStatusCommand m_editPrigIndirectionSettingsBeforeQueryStatusCommand;
        public ICommand EditPrigIndirectionSettingsBeforeQueryStatusCommand
        {
            get
            {
                if (m_editPrigIndirectionSettingsBeforeQueryStatusCommand == null)
                    m_editPrigIndirectionSettingsBeforeQueryStatusCommand = BuildUpPackageCommand(new EditPrigIndirectionSettingsBeforeQueryStatusCommand(this));
                return m_editPrigIndirectionSettingsBeforeQueryStatusCommand;
            }
        }

        OnBuildDoneCommand m_onBuildDoneCommand;
        public ICommand OnBuildDoneCommand
        {
            get
            {
                if (m_onBuildDoneCommand == null)
                    m_onBuildDoneCommand = BuildUpPackageCommand(new OnBuildDoneCommand(this));
                return m_onBuildDoneCommand;
            }
        }

        ProjectRemovedCommand m_projectRemovedCommand;
        public ICommand ProjectRemovedCommand
        {
            get
            {
                if (m_projectRemovedCommand == null)
                    m_projectRemovedCommand = BuildUpPackageCommand(new ProjectRemovedCommand(this));
                return m_projectRemovedCommand;
            }
        }
    }
}
