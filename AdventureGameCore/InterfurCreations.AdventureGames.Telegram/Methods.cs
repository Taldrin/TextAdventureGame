using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InterfurCreations.AdventureGames.Telegram.DataObjects;

namespace InterfurCreations.AdventureGames.Telegram
{
    public static class Methods
    {
        public static async Task<User> getMe(TelegramService service)
        {
            return await service.PostRequestAsync<User>("getMe", null);
        }

        public static async Task sendMessage(TelegramService service, string message, long chatId)
        {
            await service.PostRequestAsync<object>("sendMessage", new MessageSend { chat_id = chatId, text = message });
        }

        public static async Task sendMessageWithReplyOptions(TelegramService service, string message, long chatId, List<string> replyOptions)
        {
            var keyboardReply = generateKeyboard(replyOptions);
            if (keyboardReply != null)
                await service.PostRequestAsync<object>("sendMessage", new MessageSendWithKeyboard { chat_id = chatId, text = message, reply_markup = keyboardReply });
            else
                await service.PostRequestAsync<object>("sendMessage", new MessageSend { chat_id = chatId, text = message });

        }

        public static async Task sendImageWithReplyOptions(TelegramService service, string imageUrl, long chatId, List<string> replyOptions)
        {
            var keyboardReply = generateKeyboard(replyOptions);
            await service.PostRequestAsync<object>("sendPhoto", new PhotoSendWithKeyboard { chat_id = chatId, photo = imageUrl, reply_markup = keyboardReply });
        }

        private static ReplyKeyboardMarkup generateKeyboard(List<string> replyOptions)
        {
            if (replyOptions == null || replyOptions.Count == 0) return null;
            int buttonCount = 0;

            var keyboardReply = new ReplyKeyboardMarkup();
            keyboardReply.keyboard = new List<List<KeyboardButton>>();
            var currentList = new List<KeyboardButton> { new KeyboardButton { text = replyOptions.First() } };
            foreach (var replyOption in replyOptions.Skip(1))
            {
                if(replyOption.Length > 38)
                {
                    buttonCount = 1;
                    keyboardReply.keyboard.Add(currentList);
                    currentList = new List<KeyboardButton> { new KeyboardButton { text = replyOption } };
                }
                else if (replyOption.Length > 19)
                {
                    buttonCount = 1;
                    keyboardReply.keyboard.Add(currentList);
                    currentList = new List<KeyboardButton> { new KeyboardButton { text = replyOption } };
                }
                else
                {
                    buttonCount++;
                    if (buttonCount == 3)
                    {
                        buttonCount = 0;
                        keyboardReply.keyboard.Add(currentList);
                        currentList = new List<KeyboardButton> { new KeyboardButton { text = replyOption } };
                    }
                    else
                        currentList.Add(new KeyboardButton { text = replyOption });
                }
            }
            if(currentList.Count > 0)
            {
                keyboardReply.keyboard.Add(currentList);
            }

            //var keyboardReply = new ReplyKeyboardMarkup();
            //keyboardReply.keyboard = new List<List<KeyboardButton>>();

            //for (int i = 0; i < (int)replyOptions.Count; i = i + 3)
            //{
            //    var list = new List<KeyboardButton>();
            //    for (int x = 0; x < 3 && x + i < replyOptions.Count; x++)
            //    {
            //        list.Add(new KeyboardButton { text = replyOptions[i + x] });
            //    }
            //    keyboardReply.keyboard.Add(list);
            //}

            return keyboardReply;
        }

        public static async Task<List<Result>> getUpdatesAsync(TelegramService service, long updateId)
        {
            UpdateResult result = null;
            await Retry.Do(async () =>
            {
                result = await service.PostRequestAsync<UpdateResult>("getUpdates", new GetUpdates { offset = updateId, timeout = 10 });
                if (result == null || result.result == null)
                {
                    throw new Exception("GetUpdates returned null");
                }
            }, TimeSpan.FromSeconds(0.25), 10);

            if(result == null)
            {
                throw new Exception("GetUpdates returned null");
            }

            return result.result;
        }

        public static List<Result> getUpdates(TelegramService service, long updateId)
        {
            var result = service.PostRequestAsync<UpdateResult>("getUpdates", new GetUpdates { offset = updateId, timeout = 10 }).Result;
            if(result == null || result.result == null)
            {
                throw new System.Exception("GetUpdates returned null");
            }
            return result.result;
        }
    }
}
