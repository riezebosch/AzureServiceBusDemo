using System.Threading.Tasks;
using Azure.Messaging.ServiceBus.Administration;

namespace EDA.ServiceBus
{
    public static class ServiceBusAdministrationClientExt
    {
        public static async Task Setup(this ServiceBusAdministrationClient admin, string topic, string subscription, string @event)
        {
            await admin.Setup(topic);
            await admin.Setup(topic, subscription);
            await admin.Delete(topic, subscription, "$Default");
            await admin.SetupRule(topic, subscription, @event);
        }

        private static async Task Setup(this ServiceBusAdministrationClient admin, string topic)
        {
            if (!(await admin.TopicExistsAsync(topic)).Value)
            {
                await admin.CreateTopicAsync(new CreateTopicOptions(topic));
            }
        }

        private static async Task Setup(this ServiceBusAdministrationClient admin, string topic, string subscription)
        {
            if (!(await admin.SubscriptionExistsAsync(topic, subscription)).Value)
            {
                await admin.CreateSubscriptionAsync(topic, subscription);
            }
        }
        
        private static async Task Delete(this ServiceBusAdministrationClient admin, string topic, string subscription, string rule)
        {
            if ((await admin.RuleExistsAsync(topic, subscription, rule)).Value)
            {
                await admin.DeleteRuleAsync(topic, subscription, rule);
            }
        }

        private static async Task SetupRule(this ServiceBusAdministrationClient admin, string topic, string subscription, string @event)
        {
            if (!(await admin.RuleExistsAsync(topic, subscription, @event)).Value)
            {
                await admin.CreateRuleAsync(topic, subscription,
                    new CreateRuleOptions(@event, new CorrelationRuleFilter { Subject = @event }));
            }
        }
    }
}