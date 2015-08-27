using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Web.Script.Services;
using System.Text.RegularExpressions;
using System.Data;
using System.ComponentModel;

public partial class Admin : System.Web.UI.Page
{
    private static List<string> Agents = new List<string>();
    private static List<string> Queues = new List<string>();
    private static List<AgentQueueInfo> AgentQueueData = new List<AgentQueueInfo>();
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            AgentQueueData = LoadAgentInfo(@"~/AgentInfo.xml");

            foreach (AgentQueueInfo item in AgentQueueData)
            {
                Agents.Add(item.Agent);
                Queues.Add(item.Queue);
            }

            Agents = Agents.Distinct().ToList();
            Queues = LoadQueues(Queues);
        }
    }

    private List<AgentQueueInfo> LoadAgentInfo (string xmlPath)
    {
        List<AgentQueueInfo> agentQueueInfo = new List<AgentQueueInfo>();
        
        XDocument agentQueueDoc = XDocument.Load(Server.MapPath(xmlPath));
        IEnumerable<XElement> agentElement = agentQueueDoc.Descendants("Agent");

        foreach (XElement item in agentElement)
        {
            AgentQueueInfo tempagent = new AgentQueueInfo
            {
                Agent = item.Attribute("id").Value.ToString(),
                Queue = item.Attribute("queues").Value.ToString(),
                Status = item.Attribute("status").Value.ToString()
            };
            agentQueueInfo.Add(tempagent);
        }
        return agentQueueInfo;
    }

    private List<string> LoadQueues (List<string> QueueList)
    {
        List<string> queueInfo = new List<string>();

        foreach (string item in QueueList)
        {
            string[] queueSplit = item.Split(',');
            foreach (string splitItem in queueSplit)
            {
                queueInfo.Add(splitItem);
            }
        }
        
        queueInfo = queueInfo.Distinct().ToList();
        return queueInfo;
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<string> GetAgentList(string pre)
    {
        List<string> agentList = new List<string>();
        foreach (string item in Queues)
        {

            if (item.StartsWith(pre))
            {
                agentList.Add(item);
            }
        }
        return agentList;
    }

    protected void txtQueueList_TextChanged(object sender, EventArgs e)
    {
        string queueSelected = txtQueueList.Text.ToString();
        List<AgentData> agentDataforQueue = LoadAgentDataforQueue(queueSelected);

        DataTable grdAgent = ConvertToDataTable(agentDataforQueue);
        grdAgentInfo.DataSource = grdAgent;
        grdAgentInfo.DataBind();
    }

    private List<AgentData> LoadAgentDataforQueue(string queueName)
    {
        List<AgentData> agentData = new List<AgentData>();
         var agentStatusData = (from agentsforqueue in AgentQueueData
                     where agentsforqueue.Queue.Split(',').Contains(queueName)
                     select new { Agent = agentsforqueue.Agent, Status = agentsforqueue.Status });

         foreach (var item in agentStatusData)
         {
             AgentData tempData = new AgentData { Agent = item.Agent, Status = item.Status };
             agentData.Add(tempData);
         }

         return agentData;
    }

    public DataTable ConvertToDataTable<T>(IList<T> data)
    {
        PropertyDescriptorCollection properties =
           TypeDescriptor.GetProperties(typeof(T));
        DataTable table = new DataTable();
        foreach (PropertyDescriptor prop in properties)
            table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        foreach (T item in data)
        {
            DataRow row = table.NewRow();
            foreach (PropertyDescriptor prop in properties)
                row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
            table.Rows.Add(row);
        }
        return table;

    }
    
}

public class AgentQueueInfo
{
    public string Agent { get; set; }
    public string Queue { get; set; }
    public string Status { get; set; }
}

public class AgentData
{
    public string Agent { get; set; }
    public string Status { get; set; }
}