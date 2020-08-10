﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PropertyBot.Common;
using PropertyBot.Interface;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace PropertyBot.Sender.Telegram
{
    public class TelegramSender : IMessageSender
    {
        private readonly ISenderDataProvider _senderDataProvider;
        private TelegramBotClient _botClient;   

        public TelegramSender(ISenderDataProvider senderDataProvider)
        {
            _senderDataProvider = senderDataProvider;
        }

        public void Init()
        {
            var token = EnvironmentConstants.TELEGRAM_API_TOKEN.GetAsMandatoryEnvironmentVariable();
            
            _botClient = new TelegramBotClient(token);
            ListenForNewUsers();
        }

        private void ListenForNewUsers()
        {
            _botClient.OnMessage += (sender, args) =>
            {
                var chat = args.Message.Chat;
                var name = chat.FirstName;
                var id = chat.Id;

                if (_senderDataProvider.Contains<TelegramUser>(user => user.ChatId == id))
                {
                    _botClient.SendTextMessageAsync(id,
                        $"{name} du bist schon registriert...");
                }
                else
                {
                    _senderDataProvider.Add(new TelegramUser(name, id));
                    _botClient.SendTextMessageAsync(id,
                        $"Hallo {name}!. Du bist jetzt registriert und bekommst neue Angebote");
                }
            };

            _botClient.StartReceiving();
        }

        public async Task SendMessages(IEnumerable<Property> properties)
        {
            var users = _senderDataProvider.GetAll<TelegramUser>();
            var propertyList = properties.ToList();

            foreach (var user in users)
            {
                foreach (var property in propertyList)
                {
                    var message = GetMessage(property);

                    try
                    {
                        await _botClient.SendPhotoAsync(user.ChatId, new InputOnlineFile(property.ImageUrl), message, GetParseMode(property.MessageFormat));
                    }
                    catch (Exception e)
                    {
                        await _botClient.SendTextMessageAsync(user.ChatId, $"ERROR: Could not send the following message as {property.MessageFormat}: \n {message} \n\n Exception: {e.Message}\n\n{e.StackTrace}");
                    }
                    
                    await Task.Delay(1500);
                }
            }
        }

        private string GetMessage(Property property)
        {
            switch (property.MessageFormat)
            {
                case MessageFormat.Html:
                    return GetHtmlMessage(property);
                case MessageFormat.Markdown:
                    throw new NotImplementedException("Markdown format for telegram is not implemented");
                default:
                    throw new ArgumentOutOfRangeException(nameof(property.MessageFormat), property.MessageFormat, null);
            }
        }

        private string GetHtmlMessage(Property property)
        {
            var normalizedDescription = property
                .Description
                .Replace("<br>", " \n ")
                .Replace("<span>", string.Empty)
                .Replace("</span>", string.Empty);

            return $"{normalizedDescription} \n\n <i>Preis: {property.Price} €</i> \n\n <a href=\"{property.DetailsUrl}\">Mehr...</a>";
        }

        private ParseMode GetParseMode(MessageFormat messageFormat)
        {
            switch (messageFormat)
            {
                case MessageFormat.Html:
                    return ParseMode.Html;
                case MessageFormat.Markdown:
                    return ParseMode.MarkdownV2;
                default:
                    return ParseMode.Default;
            }
        }
    }
}
