using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

class Program
{
    // Это клиент для работы с Telegram Bot API, который позволяет отправлять сообщения, управлять ботом, подписываться на обновления и многое другое.
    private static ITelegramBotClient _botClient;

    // Это объект с настройками работы бота. Здесь мы будем указывать, какие типы Update мы будем получать, Timeout бота и так далее.
    private static ReceiverOptions _receiverOptions;

    static async Task Main()
    {

        _botClient = new TelegramBotClient("6672620807:AAG1JnzFdmU9W9kiw3ujA2zq0iC1CdyqXsg"); // Присваиваем нашей переменной значение, в параметре передаем Token, полученный от BotFather
        _receiverOptions = new ReceiverOptions // Также присваем значение настройкам бота
        {
            AllowedUpdates = new[] // Тут указываем типы получаемых Update`ов, о них подробнее расказано тут https://core.telegram.org/bots/api#update
            {
                UpdateType.Message, // Сообщения (текст, фото/видео, голосовые/видео сообщения и т.д.)
            },
            // Параметр, отвечающий за обработку сообщений, пришедших за то время, когда ваш бот был оффлайн
            // True - не обрабатывать, False (стоит по умолчанию) - обрабаывать
            ThrowPendingUpdates = true,
        };

        using var cts = new CancellationTokenSource();

        // UpdateHander - обработчик приходящих Update`ов
        // ErrorHandler - обработчик ошибок, связанных с Bot API
        _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cts.Token); // Запускаем бота

        var me = await _botClient.GetMeAsync(); // Создаем переменную, в которую помещаем информацию о нашем боте.
        Console.WriteLine($"{me.FirstName} запущен!");
        
        await Task.Delay(-1); // Устанавливаем бесконечную задержку, чтобы наш бот работал постоянно
    }

private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // Обязательно ставим блок try-catch, чтобы наш бот не "падал" в случае каких-либо ошибок
        try
        {
            // Сразу же ставим конструкцию switch, чтобы обрабатывать приходящие Update
            switch (update.Type)
            {
                case UpdateType.Message:
                {
                    // эта переменная будет содержать в себе все связанное с сообщениями
                    var message = update.Message;
                    
                    // From - это от кого пришло сообщение (или любой другой Update)
                    var user = message.From;
                    
                    // Chat - содержит всю информацию о чате
                    var chat = message.Chat;
                    
                    // Выводим на экран то, что пишут нашему боту, а также небольшую информацию об отправителе
                    Console.WriteLine($"{user.FirstName} ({user.Id}) написал сообщение: {message.Text}");
                    
                    if(message.Text == "/start")
                    {
                        var inlineKeyBoard = new InlineKeyboardMarkup(
                            new List<InlineKeyboardButton[]>()
                            {
                                new InlineKeyboardButton[]
                                {
                                    InlineKeyboardButton.WithUrl("Криптоман и Я", "https://t.me/Cryptoman_i_ya"),
                                }
                            }
                        );
                        await botClient.SendTextMessageAsync(
                            chat.Id,
                            "Click to me!",
                            replyMarkup: inlineKeyBoard); // Все клавиатуры передаются в параметр replyMarkup

                        }
                        else
                    { 
                        // Выводим на экран то, что пишут нашему боту, а также небольшую информацию об отправителе
                        Console.WriteLine($"{user.FirstName} ({user.Id}) написал сообщение: {message.Text}");

                        var replyKeyboard = new ReplyKeyboardMarkup(
                            new List<KeyboardButton[]>()
                            {
                            new KeyboardButton[]
                            {
                                new KeyboardButton("/start")
                            },
                        })
                        {
                                // автоматическое изменение размера клавиатуры, если не стоит true,
                                // тогда клавиатура растягивается чуть ли не до луны,
                                // проверить можете сами
                            ResizeKeyboard = true,
                        };
                            await botClient.SendTextMessageAsync(
                            chat.Id,
                            message.Text, // отправляем то, что написал пользователь
                            replyMarkup: replyKeyboard,
                            replyToMessageId: message.MessageId // по желанию можем поставить этот параметр, отвечающий за "ответ" на сообщение
                            );
                    }
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    private static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
    {
        // Тут создадим переменную, в которую поместим код ошибки и её сообщение 
        var ErrorMessage = error switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => error.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
}

