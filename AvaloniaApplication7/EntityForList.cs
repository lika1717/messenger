using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication7
{
	class EntityForList
	{
		public string Id { get; private set; }
		public string Name { get;private set; }
		public string Chat { get; private set; }
		
		public EntityForList(string id, string name, string chat)
		{
			Id = id;
			Name = name;
			Chat = chat;
		}
		public void AddChat(string message)
		{
			Chat += message+"\n";
		}
		
	}
}
