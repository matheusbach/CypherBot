using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace CypherBot
{
	internal static class OnAction
	{
		public static void BotClient_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs messageEvent)
		{
			try

			{
				NewMessageArrivalAsync(sender, messageEvent);
			}
			catch (Exception e)

			{
				Console.WriteLine(e.Message);
			}
		}

		public static void BotClient_OnInlineQuery(object sender, Telegram.Bot.Args.InlineQueryEventArgs inlineQueryEvent)
		{
			Console.WriteLine("query");

			InlineQuery inlineQuery = inlineQueryEvent.InlineQuery;

			Console.WriteLine(DateTime.UtcNow.ToShortTimeString() + " - InlineQuery - [" + inlineQuery.From.Id + "][" + Tools.GetFullName(inlineQuery.From) + "] : " + inlineQuery.Query);
		}

		public static void BotClient_OnCallbackQuery(object sender, Telegram.Bot.Args.CallbackQueryEventArgs callbackQueryEvent)
		{
			try

			{
				NewCallbackQueryAsync(sender, callbackQueryEvent);
			}
			catch (Exception e)

			{
				Console.WriteLine(e.Message);
			}
		}

		public static async Task NewCallbackQueryAsync(object sender, Telegram.Bot.Args.CallbackQueryEventArgs callbackQueryEvent)

		{
			CallbackQuery callbackQuery = callbackQueryEvent.CallbackQuery;

			if (Data.ChannelPosts.posts.ContainsKey(callbackQuery.Message.Chat.Id + ":" + callbackQuery.Message.MessageId))

			{
				Console.WriteLine(callbackQuery.Message.Chat.Id + callbackQuery.Message.MessageId);

				switch (callbackQuery.Data)

				{
					case "ChannelPostLike":

						{
							Console.WriteLine("like");

							if (!Data.ChannelPosts.posts[callbackQuery.Message.Chat.Id + ":" + callbackQuery.Message.MessageId].PostLikes.Contains(callbackQuery.From.Id))

							{
								Data.ChannelPosts.posts[callbackQuery.Message.Chat.Id + ":" + callbackQuery.Message.MessageId].PostLikes.Add(callbackQuery.From.Id);

								if (Data.ChannelPosts.posts[callbackQuery.Message.Chat.Id + ":" + callbackQuery.Message.MessageId].PostDislikes.Contains(callbackQuery.From.Id))

								{
									Data.ChannelPosts.posts[callbackQuery.Message.Chat.Id + ":" + callbackQuery.Message.MessageId].PostDislikes.Remove(callbackQuery.From.Id);
								}
							}
							else

							{
								Data.ChannelPosts.posts[callbackQuery.Message.Chat.Id + ":" + callbackQuery.Message.MessageId].PostLikes.Remove(callbackQuery.From.Id);
							}
						}

						break;

					case "ChannelPostDislike":

						{
							Console.WriteLine("like");

							if (!Data.ChannelPosts.posts[callbackQuery.Message.Chat.Id + ":" + callbackQuery.Message.MessageId].PostDislikes.Contains(callbackQuery.From.Id))

							{
								Data.ChannelPosts.posts[callbackQuery.Message.Chat.Id + ":" + callbackQuery.Message.MessageId].PostDislikes.Add(callbackQuery.From.Id);

								if (Data.ChannelPosts.posts[callbackQuery.Message.Chat.Id + ":" + callbackQuery.Message.MessageId].PostLikes.Contains(callbackQuery.From.Id))

								{
									Data.ChannelPosts.posts[callbackQuery.Message.Chat.Id + ":" + callbackQuery.Message.MessageId].PostLikes.Remove(callbackQuery.From.Id);
								}
							}
							else

							{
								Data.ChannelPosts.posts[callbackQuery.Message.Chat.Id + ":" + callbackQuery.Message.MessageId].PostDislikes.Remove(callbackQuery.From.Id);
							}
						}

						break;
				}

				CypherBot.botClient.EditMessageReplyMarkupAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId, Resources.Messages.replyMarkupChannelRating(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId));

				Data.ChannelPosts.SavePostsData();

				if (Data.ChannelPosts.posts[callbackQuery.Message.Chat.Id + ":" + callbackQuery.Message.MessageId].PostLikes.Count + Data.ChannelPosts.posts[callbackQuery.Message.Chat.Id + ":" + callbackQuery.Message.MessageId].PostDislikes.Count > 5 && Data.ChannelPosts.posts[callbackQuery.Message.Chat.Id + ":" + callbackQuery.Message.MessageId].PostDislikes.Count > Data.ChannelPosts.posts[callbackQuery.Message.Chat.Id + ":" + callbackQuery.Message.MessageId].PostLikes.Count)

				{
					Message forwardedMessage = await CypherBot.botClient.ForwardMessageAsync(Props.moderatorGroupChatId, callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);

					CypherBot.botClient.SendTextMessageAsync(Props.moderatorGroupChatId, "Post apagado por avaliação negativa\n*" + Data.ChannelPosts.posts[callbackQuery.Message.Chat.Id + ":" + callbackQuery.Message.MessageId].PostLikes.Count + "* likes, *" + Data.ChannelPosts.posts[callbackQuery.Message.Chat.Id + ":" + callbackQuery.Message.MessageId].PostDislikes.Count + "* deslike", Telegram.Bot.Types.Enums.ParseMode.MarkdownV2, true, false, forwardedMessage.MessageId);

					CypherBot.botClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
				}
			}

			Console.WriteLine(DateTime.UtcNow.ToShortTimeString() + " - InlineQuery - [" + callbackQuery.From.Id + "][" + Tools.GetFullName(callbackQuery.From) + "] : " + callbackQuery.Data);
		}

		public static async Task NewMessageArrivalAsync(object sender, Telegram.Bot.Args.MessageEventArgs messageEvent)

		{
			Message message = messageEvent.Message;

			if (message.Type == Telegram.Bot.Types.Enums.MessageType.Text && message.Text != null)
			{
				Console.WriteLine(DateTime.UtcNow.ToShortTimeString() + " - [" + message.From.Id + "][" + (Tools.GetFullName(message.From) + "] : " + message.Text));

				if (message.Text.StartsWith("/sorteio", StringComparison.OrdinalIgnoreCase))

				{
					string messageToSend = null;

					if (message.Text.Equals("/sorteio_end", StringComparison.OrdinalIgnoreCase) && message.From.Id == 366723664)

					{
						if (!System.IO.File.Exists(Directory.GetCurrentDirectory() + @"/data/SorteioEnded"))

						{
							System.IO.File.Create(Directory.GetCurrentDirectory() + @"/data/SorteioEnded");
						}

						Message send0 = await CypherBot.botClient.SendDocumentAsync(message.Chat.Id, Tools.GetparticipantesList(), "Lista de participantes", Telegram.Bot.Types.Enums.ParseMode.Default, true, message.MessageId);

						Message send1 = await CypherBot.botClient.SendTextMessageAsync(message.Chat.Id, Tools.StringForkMarkdownUse("Participações no sorteio enceradas. Lista de participantes gerada. Boa sorte a todos os participantes. Aguardem o anúncio do sorteio"), Telegram.Bot.Types.Enums.ParseMode.MarkdownV2, true, true, message.MessageId);

						CypherBot.botClient.PinChatMessageAsync(send1.Chat.Id, send1.MessageId);
					}
					else if (message.Text.StartsWith("/sorteio_list", StringComparison.OrdinalIgnoreCase) | message.Text.StartsWith("/sorteio_list@cypherpunksbrbot", StringComparison.OrdinalIgnoreCase))

					{
						Message sentedMessage1 = await CypherBot.botClient.SendDocumentAsync(message.Chat.Id, Tools.GetparticipantesList(), "Lista de participantes", Telegram.Bot.Types.Enums.ParseMode.Default, true, message.MessageId);

						if (message.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Group || message.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Supergroup)

						{
							Data.MessagesToDelete.messagesToDelete.Add(new Data.MessageForDelete() { ChatId = message.Chat.Id, MessageId = message.MessageId, MessageExpire = DateTime.UtcNow.AddSeconds(10) });

							if (message.Text != "/sorteio_list_keep")

							{
								Data.MessagesToDelete.messagesToDelete.Add(new Data.MessageForDelete() { ChatId = sentedMessage1.Chat.Id, MessageId = sentedMessage1.MessageId, MessageExpire = DateTime.UtcNow.AddSeconds(10) });
							}
						}
					}
					else if (message.Text.Equals("/sorteio", StringComparison.OrdinalIgnoreCase) | message.Text.Equals("/sorteio@cypherpunksbrbot", StringComparison.OrdinalIgnoreCase))

					{
						if (System.IO.File.Exists(Directory.GetCurrentDirectory() + @"/data/SorteioEnded"))

						{
							messageToSend = Tools.StringForkMarkdownUse("O tempo de participação do sorteio acabou. Aguarde por novos anúncios");
						}
						else

						{
							if (Data.SorteioParticipantes.participantes.ContainsKey(message.From.Id))

							{
								if (!Data.SorteioParticipantes.participantes[message.From.Id].IsValid)

								{
									messageToSend = Tools.StringForkMarkdownUse("Você está banido do sorteio");
								}
								else

								{
									messageToSend = Tools.StringForkMarkdownUse("Você já está participando do sorteio");
								}
							}
							else

							{
								ChatMember chatMember = await CypherBot.botClient.GetChatMemberAsync(Props.grupoPrincipalChatId, message.From.Id);

								if ((chatMember.Status == Telegram.Bot.Types.Enums.ChatMemberStatus.Administrator || chatMember.Status == Telegram.Bot.Types.Enums.ChatMemberStatus.Creator || chatMember.Status == Telegram.Bot.Types.Enums.ChatMemberStatus.Member) && !(chatMember.Status == Telegram.Bot.Types.Enums.ChatMemberStatus.Restricted || chatMember.Status == Telegram.Bot.Types.Enums.ChatMemberStatus.Kicked || chatMember.Status == Telegram.Bot.Types.Enums.ChatMemberStatus.Left))

								{
									Data.Participante participante = new Data.Participante()

									{
										Name = Tools.GetFullName(message.From),

										IsValid = true,

										DateTime = DateTime.UtcNow
									};

									Data.SorteioParticipantes.participantes.TryAdd(message.From.Id, participante);

									messageToSend = Tools.StringForkMarkdownUse("Agora você está participando do sorteio. Boa sorte");
								}
								else

								{
									messageToSend = Tools.StringForkMarkdownUse("Você precisa estar em nosso grupo para participar do sorteio.") + "\n*Link:* [Clique aqui para ver o link](t.me/criptologia/2)";
								}
							}
						}
					}

					Console.WriteLine(messageToSend);

					Message sentedMessage = await CypherBot.botClient.SendTextMessageAsync(message.Chat.Id, messageToSend, Telegram.Bot.Types.Enums.ParseMode.MarkdownV2, true, true, message.MessageId);

					if (message.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Group || message.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Supergroup)

					{
						Data.MessagesToDelete.messagesToDelete.Add(new Data.MessageForDelete() { ChatId = message.Chat.Id, MessageId = message.MessageId, MessageExpire = DateTime.UtcNow.AddSeconds(10) });

						Data.MessagesToDelete.messagesToDelete.Add(new Data.MessageForDelete() { ChatId = sentedMessage.Chat.Id, MessageId = sentedMessage.MessageId, MessageExpire = DateTime.UtcNow.AddSeconds(10) });
					}
				}
				else if (message.Text.StartsWith("/off", StringComparison.OrdinalIgnoreCase))

				{
					string messageToSend = Resources.Messages.offtopic_request;

					Console.WriteLine(messageToSend);

					Message sentedMessage = await CypherBot.botClient.SendTextMessageAsync(message.Chat.Id, messageToSend, Telegram.Bot.Types.Enums.ParseMode.MarkdownV2, true, true, message.MessageId);

					if (message.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Group || message.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Supergroup)

					{
						Data.MessagesToDelete.messagesToDelete.Add(new Data.MessageForDelete() { ChatId = message.Chat.Id, MessageId = message.MessageId, MessageExpire = DateTime.UtcNow.AddSeconds(100) });

						Data.MessagesToDelete.messagesToDelete.Add(new Data.MessageForDelete() { ChatId = sentedMessage.Chat.Id, MessageId = sentedMessage.MessageId, MessageExpire = DateTime.UtcNow.AddSeconds(100) });
					}
				}
				else if (message.Text.Contains("/getUserInfo", StringComparison.OrdinalIgnoreCase))

				{
					if (message.ReplyToMessage == null) { Console.WriteLine("null reply"); }
					else

					{
						ChatMember messageToSend = await CypherBot.botClient.GetChatMemberAsync(message.Chat.Id, message.ReplyToMessage.From.Id); // precisa aguardar pelo retorno para gerar a proxima mensagem

						Console.WriteLine(messageToSend.Status.ToString());

						Message sentedMessage = await CypherBot.botClient.SendTextMessageAsync(message.Chat.Id, messageToSend.UntilDate.ToString(), Telegram.Bot.Types.Enums.ParseMode.Default, true, true, message.MessageId);

						if (message.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Group || message.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Supergroup)

						{
							Data.MessagesToDelete.messagesToDelete.Add(new Data.MessageForDelete() { ChatId = message.Chat.Id, MessageId = message.MessageId, MessageExpire = DateTime.UtcNow.AddSeconds(10) });

							Data.MessagesToDelete.messagesToDelete.Add(new Data.MessageForDelete() { ChatId = sentedMessage.Chat.Id, MessageId = sentedMessage.MessageId, MessageExpire = DateTime.UtcNow.AddSeconds(10) });
						}
					}
				}
				else if (message.Text.StartsWith("/post", StringComparison.OrdinalIgnoreCase))
				{
					string messageToSend = "Sua postagem foi enviada para o [canal](https://t.me/CypherpunksBrasil)";

					if (message.ReplyToMessage == null)

					{
						messageToSend = "Para enviar um post para nosso canal você precisa enviar esse comando como resposta na mensagem que deseja encaminhar";
					}
					else

					{
						Data.Post postData = new Data.Post()
						{
							forwardedFrom = message.ReplyToMessage.IsForwarded ? message.ReplyToMessage.ForwardFrom.Id : 0,

							sentBy = message.ReplyToMessage.From.Id,

							recomendedBy = message.From.Id,

							PostLikes = new List<Int64>(),

							PostDislikes = new List<Int64>()
						};

						switch (message.ReplyToMessage.Type)

						{
							case Telegram.Bot.Types.Enums.MessageType.Text:

								{
									StringBuilder postText = new StringBuilder();

									postText.AppendLine(Tools.StringForkMarkdownUse(message.ReplyToMessage.Text));

									postText.AppendLine(Tools.StringForkMarkdownUse("\n︻╦╤─  -  -  -  -  -  -  -  -  -  -  - "));

									postText.AppendLine(Tools.GetChannelPostDescription(message));

									Message postMessage = await CypherBot.botClient.SendTextMessageAsync(Props.postChannelChatId, postText.ToString(), Telegram.Bot.Types.Enums.ParseMode.MarkdownV2, false, false, 0, Resources.Messages.replyMarkupChannelRating());

									Data.ChannelPosts.posts.TryAdd(postMessage.Chat.Id + ":" + postMessage.MessageId, postData);

									Data.MessagesToDelete.messagesToDelete.Add(new Data.MessageForDelete() { ChatId = message.ReplyToMessage.Chat.Id, MessageId = message.ReplyToMessage.MessageId, MessageExpire = DateTime.UtcNow.AddSeconds(1200) });
								}

								break;

							case Telegram.Bot.Types.Enums.MessageType.Document:

								{
									StringBuilder postText = new StringBuilder();

									postText.AppendLine(Tools.StringForkMarkdownUse(message.ReplyToMessage.Caption));

									postText.AppendLine(Tools.StringForkMarkdownUse("\n︻╦╤─  -  -  -  -  -  -  -  -  -  -  -  -  -"));

									postText.AppendLine(Tools.GetChannelPostDescription(message));

									Message postMessage = await CypherBot.botClient.SendDocumentAsync(Props.postChannelChatId, message.ReplyToMessage.Document.FileId, postText.ToString(), Telegram.Bot.Types.Enums.ParseMode.MarkdownV2, false, 0, Resources.Messages.replyMarkupChannelRating());

									Data.ChannelPosts.posts.TryAdd(postMessage.Chat.Id + ":" + postMessage.MessageId, postData);

									Data.MessagesToDelete.messagesToDelete.Add(new Data.MessageForDelete() { ChatId = message.ReplyToMessage.Chat.Id, MessageId = message.ReplyToMessage.MessageId, MessageExpire = DateTime.UtcNow.AddSeconds(1200) });
								}

								break;

							case Telegram.Bot.Types.Enums.MessageType.Photo:

								{
									StringBuilder postText = new StringBuilder();

									postText.AppendLine(Tools.StringForkMarkdownUse(message.ReplyToMessage.Caption));

									postText.AppendLine(Tools.StringForkMarkdownUse("\n︻╦╤─  -  -  -  -  -  -  -  -  -  -  -  -  -"));

									postText.AppendLine(Tools.GetChannelPostDescription(message));

									Message postMessage = await CypherBot.botClient.SendPhotoAsync(Props.postChannelChatId, message.ReplyToMessage.Photo[0].FileId, postText.ToString(), Telegram.Bot.Types.Enums.ParseMode.MarkdownV2, false, 0, Resources.Messages.replyMarkupChannelRating());

									Data.ChannelPosts.posts.TryAdd(postMessage.Chat.Id + ":" + postMessage.MessageId, postData);

									Data.MessagesToDelete.messagesToDelete.Add(new Data.MessageForDelete() { ChatId = message.ReplyToMessage.Chat.Id, MessageId = message.ReplyToMessage.MessageId, MessageExpire = DateTime.UtcNow.AddSeconds(1200) });
								}

								break;

							case Telegram.Bot.Types.Enums.MessageType.Video:

								{
									StringBuilder postText = new StringBuilder();

									postText.AppendLine(Tools.StringForkMarkdownUse(message.ReplyToMessage.Caption));

									postText.AppendLine(Tools.StringForkMarkdownUse("\n︻╦╤─  -  -  -  -  -  -  -  -  -  -  -  -  -"));

									postText.AppendLine(Tools.GetChannelPostDescription(message));

									Message postMessage = await CypherBot.botClient.SendVideoAsync(Props.postChannelChatId, message.ReplyToMessage.Video.FileId, message.ReplyToMessage.Video.Duration, message.ReplyToMessage.Video.Width, message.ReplyToMessage.Video.Height, postText.ToString(), Telegram.Bot.Types.Enums.ParseMode.MarkdownV2, false, false, 0, Resources.Messages.replyMarkupChannelRating());

									Data.ChannelPosts.posts.TryAdd(postMessage.Chat.Id + ":" + postMessage.MessageId, postData);

									Data.MessagesToDelete.messagesToDelete.Add(new Data.MessageForDelete() { ChatId = message.ReplyToMessage.Chat.Id, MessageId = message.ReplyToMessage.MessageId, MessageExpire = DateTime.UtcNow.AddSeconds(1200) });
								}

								break;

							case Telegram.Bot.Types.Enums.MessageType.Audio:

								{
									StringBuilder postText = new StringBuilder();

									postText.AppendLine(Tools.StringForkMarkdownUse(message.ReplyToMessage.Caption));

									postText.AppendLine(Tools.StringForkMarkdownUse("\n︻╦╤─  -  -  -  -  -  -  -  -  -  -  -  -  -"));

									postText.AppendLine(Tools.GetChannelPostDescription(message));

									Message postMessage = await CypherBot.botClient.SendAudioAsync(Props.postChannelChatId, message.ReplyToMessage.Audio.FileId, postText.ToString(), Telegram.Bot.Types.Enums.ParseMode.MarkdownV2, message.ReplyToMessage.Audio.Duration, message.ReplyToMessage.Audio.Performer, message.ReplyToMessage.Audio.Title, false, 0, Resources.Messages.replyMarkupChannelRating());

									Data.ChannelPosts.posts.TryAdd(postMessage.Chat.Id + ":" + postMessage.MessageId, postData);

									Data.MessagesToDelete.messagesToDelete.Add(new Data.MessageForDelete() { ChatId = message.ReplyToMessage.Chat.Id, MessageId = message.ReplyToMessage.MessageId, MessageExpire = DateTime.UtcNow.AddSeconds(1200) });
								}

								break;

							case Telegram.Bot.Types.Enums.MessageType.Voice:

								{
									StringBuilder postText = new StringBuilder();

									postText.AppendLine(Tools.StringForkMarkdownUse("\n︻╦╤─  -  -  -  -  -  -  -  -  -  -  -  -  -  -"));

									postText.AppendLine(Tools.GetChannelPostDescription(message));

									Message postMessage = await CypherBot.botClient.SendVoiceAsync(Props.postChannelChatId, message.ReplyToMessage.Voice.FileId, postText.ToString(), Telegram.Bot.Types.Enums.ParseMode.MarkdownV2, message.ReplyToMessage.Voice.Duration, false, 0, Resources.Messages.replyMarkupChannelRating());

									Data.ChannelPosts.posts.TryAdd(postMessage.Chat.Id + ":" + postMessage.MessageId, postData);

									Data.MessagesToDelete.messagesToDelete.Add(new Data.MessageForDelete() { ChatId = message.ReplyToMessage.Chat.Id, MessageId = message.ReplyToMessage.MessageId, MessageExpire = DateTime.UtcNow.AddSeconds(1200) });
								}

								break;

							default:

								{
									messageToSend = "Mensagem não suportada";
								}

								break;
						}
					}

					Console.WriteLine(messageToSend);

					Message sentedMessage = await CypherBot.botClient.SendTextMessageAsync(message.Chat.Id, messageToSend, Telegram.Bot.Types.Enums.ParseMode.MarkdownV2, true, true, message.MessageId);

					if (message.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Group || message.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Supergroup)
					{
						Data.MessagesToDelete.messagesToDelete.Add(new Data.MessageForDelete() { ChatId = message.Chat.Id, MessageId = message.MessageId, MessageExpire = DateTime.UtcNow.AddSeconds(600) });

						Data.MessagesToDelete.messagesToDelete.Add(new Data.MessageForDelete() { ChatId = sentedMessage.Chat.Id, MessageId = sentedMessage.MessageId, MessageExpire = DateTime.UtcNow.AddSeconds(600) });
					}
				}
				else if (message.Text.StartsWith("/")) { Data.MessagesToDelete.messagesToDelete.Add(new Data.MessageForDelete() { ChatId = message.Chat.Id, MessageId = message.MessageId, MessageExpire = DateTime.UtcNow.AddSeconds(10) }); }
				else if (message.From.Id == 807456505 && message.Text.Contains("zap", StringComparison.OrdinalIgnoreCase))
				{
					CypherBot.botClient.RestrictChatMemberAsync(message.Chat.Id, message.From.Id, new ChatPermissions() { CanAddWebPagePreviews = false, CanChangeInfo = false, CanInviteUsers = false, CanPinMessages = false, CanSendMediaMessages = false, CanSendMessages = false, CanSendOtherMessages = false, CanSendPolls = false }, DateTime.UtcNow.AddDays(1));
					CypherBot.botClient.SendTextMessageAsync(message.Chat.Id, "Usuário " + message.From.Username + " mutado por 1 dia.\nMotivo: viciado em zap", Telegram.Bot.Types.Enums.ParseMode.MarkdownV2, true, true, message.MessageId);
				}
			}
		}
	}
}