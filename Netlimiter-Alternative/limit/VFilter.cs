using NetLimiter.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Netlimiter_Alternative.Limit
{
    internal class VFilter
    {

        public Filter filter;
        public NLClient client;
        public RuleDir ruleDir;
        public Rule rule;
        public ushort port;
        public uint bytes;
        public FilterModel filterModel;
        public string filterName;

        public VFilter(NLClient client, RuleDir ruleDir, ushort port, uint bytes, FilterModel filterModel)
        {
            this.client = client;
            this.ruleDir = ruleDir;
            this.port = port;
            this.bytes = bytes;
            this.filterModel = filterModel;
            if (port > 0)
            {
                this.filterName = port.ToString() + ((ruleDir.ToString() == "Out") ? " UL" : " DL");
            }
            else
            {
                this.filterName = "Destiny 2";
            }
            HotKeyManager.RegisterHotKey(filterModel.getKeyFromString(), Program.config.modifier);

            this.rule = new LimitRule(ruleDir, bytes);

            // Checks if filter does not exist, if it doesnt exist create new filter.
            if (client.Filters.Find(x => x.Name == filterName) == null)
            {
                createFilter();
                return;
            }

            Console.WriteLine("Filter already exists - Writing filter.");
            this.filter = client.Filters.Find(x => x.Name == this.filterName);

            this.rule = client.Rules.Find(x => x.FilterId == this.filter.Id);
            this.rule.IsEnabled = false;
            client.UpdateRule(rule);
        }

        public void createFilter()
        {
            Console.WriteLine("Creating filter");
            this.filter = checkForFullGameAndAdd(Program.config.appPath);

            this.filter = client.AddFilter(this.filter);
            this.rule = client.AddRule(this.filter.Id, new LimitRule(ruleDir, bytes));
            this.rule.IsEnabled = false;
            client.UpdateRule(rule);
        }

        public Filter checkForFullGameAndAdd(String appPath)
        {
            Filter filter;
            filter = new Filter(this.filterName);
            filter.Functions.Add(new FFRemotePortInRange(new PortRangeFilterValue(port, port))); // Limit port in range
            return filter;
        }

        public void updateAppPath(String appPath)
        {
            this.client.RemoveFilter(this.filter);
            this.filter = checkForFullGameAndAdd(appPath);

            this.rule = new LimitRule(this.ruleDir, this.bytes);

            this.filter = client.AddFilter(this.filter);
            this.rule = client.AddRule(this.filter.Id, this.rule);

            this.rule.IsEnabled = false;
            this.client.UpdateRule(this.rule);
        }

        public void updateRuleBps(uint bps, RuleDir rule)
        {
            this.client.RemoveFilter(this.filter);
            this.filter = checkForFullGameAndAdd(Program.config.appPath);
            this.filter = this.client.AddFilter(this.filter);

            this.rule = new LimitRule(rule, bps);

            this.rule = this.client.AddRule(this.filter.Id, this.rule);

            this.rule.IsEnabled = false;
            this.client.UpdateRule(this.rule);
        }


        public void killConnections()
        {
            this.client.KillCnnsByFilterId(this.filter.Id);
        }
    }
}
