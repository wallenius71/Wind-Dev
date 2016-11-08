﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.269
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FindFile
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="WindART_SMI")]
	public partial class WindART_SMIDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertLog(Log instance);
    partial void UpdateLog(Log instance);
    partial void DeleteLog(Log instance);
    partial void InsertProcessFile(ProcessFile instance);
    partial void UpdateProcessFile(ProcessFile instance);
    partial void DeleteProcessFile(ProcessFile instance);
    partial void InsertFileLocation(FileLocation instance);
    partial void UpdateFileLocation(FileLocation instance);
    partial void DeleteFileLocation(FileLocation instance);
    #endregion
		
		public WindART_SMIDataContext() : 
				base(global::FindFile.Properties.Settings.Default.WindART_SMIConnectionString2, mappingSource)
		{
			OnCreated();
		}
		
		public WindART_SMIDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public WindART_SMIDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public WindART_SMIDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public WindART_SMIDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<Log> Logs
		{
			get
			{
				return this.GetTable<Log>();
			}
		}
		
		public System.Data.Linq.Table<ProcessFile> ProcessFiles
		{
			get
			{
				return this.GetTable<ProcessFile>();
			}
		}
		
		public System.Data.Linq.Table<FileLocation> FileLocations
		{
			get
			{
				return this.GetTable<FileLocation>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.[Log]")]
	public partial class Log : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _LogId;
		
		private System.Nullable<System.DateTime> _Time;
		
		private System.Nullable<byte> _LogSeverityId;
		
		private string _Process;
		
		private string _Message;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnLogIdChanging(int value);
    partial void OnLogIdChanged();
    partial void OnTimeChanging(System.Nullable<System.DateTime> value);
    partial void OnTimeChanged();
    partial void OnLogSeverityIdChanging(System.Nullable<byte> value);
    partial void OnLogSeverityIdChanged();
    partial void OnProcessChanging(string value);
    partial void OnProcessChanged();
    partial void OnMessageChanging(string value);
    partial void OnMessageChanged();
    #endregion
		
		public Log()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LogId", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int LogId
		{
			get
			{
				return this._LogId;
			}
			set
			{
				if ((this._LogId != value))
				{
					this.OnLogIdChanging(value);
					this.SendPropertyChanging();
					this._LogId = value;
					this.SendPropertyChanged("LogId");
					this.OnLogIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Time", DbType="DateTime")]
		public System.Nullable<System.DateTime> Time
		{
			get
			{
				return this._Time;
			}
			set
			{
				if ((this._Time != value))
				{
					this.OnTimeChanging(value);
					this.SendPropertyChanging();
					this._Time = value;
					this.SendPropertyChanged("Time");
					this.OnTimeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LogSeverityId", DbType="TinyInt")]
		public System.Nullable<byte> LogSeverityId
		{
			get
			{
				return this._LogSeverityId;
			}
			set
			{
				if ((this._LogSeverityId != value))
				{
					this.OnLogSeverityIdChanging(value);
					this.SendPropertyChanging();
					this._LogSeverityId = value;
					this.SendPropertyChanged("LogSeverityId");
					this.OnLogSeverityIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Process", DbType="VarChar(50)")]
		public string Process
		{
			get
			{
				return this._Process;
			}
			set
			{
				if ((this._Process != value))
				{
					this.OnProcessChanging(value);
					this.SendPropertyChanging();
					this._Process = value;
					this.SendPropertyChanged("Process");
					this.OnProcessChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Message", DbType="VarChar(1000)")]
		public string Message
		{
			get
			{
				return this._Message;
			}
			set
			{
				if ((this._Message != value))
				{
					this.OnMessageChanging(value);
					this.SendPropertyChanging();
					this._Message = value;
					this.SendPropertyChanged("Message");
					this.OnMessageChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.ProcessFile")]
	public partial class ProcessFile : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _ProcessFileId;
		
		private System.Nullable<System.DateTime> _ProcessDate;
		
		private string _OriginalName;
		
		private string _Name;
		
		private System.Nullable<int> _FileStatus;
		
		private System.Nullable<int> _FileLocationId;
		
		private System.Nullable<int> _FileFormatId;
		
		private System.Nullable<int> _SiteId;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnProcessFileIdChanging(int value);
    partial void OnProcessFileIdChanged();
    partial void OnProcessDateChanging(System.Nullable<System.DateTime> value);
    partial void OnProcessDateChanged();
    partial void OnOriginalNameChanging(string value);
    partial void OnOriginalNameChanged();
    partial void OnNameChanging(string value);
    partial void OnNameChanged();
    partial void OnFileStatusChanging(System.Nullable<int> value);
    partial void OnFileStatusChanged();
    partial void OnFileLocationIdChanging(System.Nullable<int> value);
    partial void OnFileLocationIdChanged();
    partial void OnFileFormatIdChanging(System.Nullable<int> value);
    partial void OnFileFormatIdChanged();
    partial void OnSiteIdChanging(System.Nullable<int> value);
    partial void OnSiteIdChanged();
    #endregion
		
		public ProcessFile()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ProcessFileId", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int ProcessFileId
		{
			get
			{
				return this._ProcessFileId;
			}
			set
			{
				if ((this._ProcessFileId != value))
				{
					this.OnProcessFileIdChanging(value);
					this.SendPropertyChanging();
					this._ProcessFileId = value;
					this.SendPropertyChanged("ProcessFileId");
					this.OnProcessFileIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ProcessDate", DbType="DateTime")]
		public System.Nullable<System.DateTime> ProcessDate
		{
			get
			{
				return this._ProcessDate;
			}
			set
			{
				if ((this._ProcessDate != value))
				{
					this.OnProcessDateChanging(value);
					this.SendPropertyChanging();
					this._ProcessDate = value;
					this.SendPropertyChanged("ProcessDate");
					this.OnProcessDateChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_OriginalName", DbType="VarChar(8000)")]
		public string OriginalName
		{
			get
			{
				return this._OriginalName;
			}
			set
			{
				if ((this._OriginalName != value))
				{
					this.OnOriginalNameChanging(value);
					this.SendPropertyChanging();
					this._OriginalName = value;
					this.SendPropertyChanged("OriginalName");
					this.OnOriginalNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="VarChar(8000)")]
		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FileStatus", DbType="Int")]
		public System.Nullable<int> FileStatus
		{
			get
			{
				return this._FileStatus;
			}
			set
			{
				if ((this._FileStatus != value))
				{
					this.OnFileStatusChanging(value);
					this.SendPropertyChanging();
					this._FileStatus = value;
					this.SendPropertyChanged("FileStatus");
					this.OnFileStatusChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FileLocationId", DbType="Int")]
		public System.Nullable<int> FileLocationId
		{
			get
			{
				return this._FileLocationId;
			}
			set
			{
				if ((this._FileLocationId != value))
				{
					this.OnFileLocationIdChanging(value);
					this.SendPropertyChanging();
					this._FileLocationId = value;
					this.SendPropertyChanged("FileLocationId");
					this.OnFileLocationIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FileFormatId", DbType="Int")]
		public System.Nullable<int> FileFormatId
		{
			get
			{
				return this._FileFormatId;
			}
			set
			{
				if ((this._FileFormatId != value))
				{
					this.OnFileFormatIdChanging(value);
					this.SendPropertyChanging();
					this._FileFormatId = value;
					this.SendPropertyChanged("FileFormatId");
					this.OnFileFormatIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SiteId", DbType="Int")]
		public System.Nullable<int> SiteId
		{
			get
			{
				return this._SiteId;
			}
			set
			{
				if ((this._SiteId != value))
				{
					this.OnSiteIdChanging(value);
					this.SendPropertyChanging();
					this._SiteId = value;
					this.SendPropertyChanged("SiteId");
					this.OnSiteIdChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.FileLocation")]
	public partial class FileLocation : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _FileLocationId;
		
		private string _Folder;
		
		private string _FileMask;
		
		private System.Nullable<bool> _Recurse;
		
		private System.Nullable<bool> _Encrypted;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnFileLocationIdChanging(int value);
    partial void OnFileLocationIdChanged();
    partial void OnFolderChanging(string value);
    partial void OnFolderChanged();
    partial void OnFileMaskChanging(string value);
    partial void OnFileMaskChanged();
    partial void OnRecurseChanging(System.Nullable<bool> value);
    partial void OnRecurseChanged();
    partial void OnEncryptedChanging(System.Nullable<bool> value);
    partial void OnEncryptedChanged();
    #endregion
		
		public FileLocation()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FileLocationId", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int FileLocationId
		{
			get
			{
				return this._FileLocationId;
			}
			set
			{
				if ((this._FileLocationId != value))
				{
					this.OnFileLocationIdChanging(value);
					this.SendPropertyChanging();
					this._FileLocationId = value;
					this.SendPropertyChanged("FileLocationId");
					this.OnFileLocationIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Folder", DbType="VarChar(1000)")]
		public string Folder
		{
			get
			{
				return this._Folder;
			}
			set
			{
				if ((this._Folder != value))
				{
					this.OnFolderChanging(value);
					this.SendPropertyChanging();
					this._Folder = value;
					this.SendPropertyChanged("Folder");
					this.OnFolderChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FileMask", DbType="VarChar(100)")]
		public string FileMask
		{
			get
			{
				return this._FileMask;
			}
			set
			{
				if ((this._FileMask != value))
				{
					this.OnFileMaskChanging(value);
					this.SendPropertyChanging();
					this._FileMask = value;
					this.SendPropertyChanged("FileMask");
					this.OnFileMaskChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Recurse", DbType="Bit")]
		public System.Nullable<bool> Recurse
		{
			get
			{
				return this._Recurse;
			}
			set
			{
				if ((this._Recurse != value))
				{
					this.OnRecurseChanging(value);
					this.SendPropertyChanging();
					this._Recurse = value;
					this.SendPropertyChanged("Recurse");
					this.OnRecurseChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Encrypted", DbType="Bit")]
		public System.Nullable<bool> Encrypted
		{
			get
			{
				return this._Encrypted;
			}
			set
			{
				if ((this._Encrypted != value))
				{
					this.OnEncryptedChanging(value);
					this.SendPropertyChanging();
					this._Encrypted = value;
					this.SendPropertyChanged("Encrypted");
					this.OnEncryptedChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591
