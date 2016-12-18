using System;

using System.Collections.Generic;

using System.Collections.ObjectModel;

using System.Diagnostics;

using System.Linq;

using System.Text;

using System.Threading.Tasks;

using Microsoft.WindowsAzure.MobileServices;

using Newtonsoft.Json;

namespace XamarinDevDay.Services

{

	public partial class ItemManager

	{

		static ItemManager defaultInstance = new ItemManager();

		MobileServiceClient client;

		IMobileServiceTable<TodoItem> todoTable;

		private ItemManager()

		{

			this.client = new MobileServiceClient(@"https://xamarindevdaysmx.azurewebsites.net");

			this.todoTable = client.GetTable<TodoItem>();

		}

		public static ItemManager DefaultManager

		{

			get

			{

				return defaultInstance;

			}

			private set

			{

				defaultInstance = value;

			}

		}

		public MobileServiceClient CurrentClient

		{

			get { return client; }

		}

		public async Task SaveTaskAsync(TodoItem item)

		{

			if (item.Id == null)

			{

				await todoTable.InsertAsync(item);

			}

			else

			{

				await todoTable.UpdateAsync(item);

			}

		}

	}

	public class TodoItem

	{

		string id;

		string name;

		bool done;

		[JsonProperty(PropertyName = "id")]

		public string Id

		{

			get { return id; }

			set { id = value; }

		}

		[JsonProperty(PropertyName = "text")]

		public string Name

		{

			get { return name; }

			set { name = value; }

		}

		[JsonProperty(PropertyName = "complete")]

		public bool Done

		{

			get { return done; }

			set { done = value; }

		}

		[Version]

		public string Version { get; set; }

	}

}