using System;

using System.IO;

using Xamarin.Forms;

using XamarinDevDay.Services;

using System.Collections.Generic;

using System.Text;

using Plugin.Connectivity;

using Plugin.Media.Abstractions;

namespace XamarinDevDay.UI

{

	public partial class StartPage : ContentPage

	{

		ItemManager manager;

		static Stream streamCopy;

		string ResultadoEmociones = "Participante: Juan"; //Escribe tu nombre

		public StartPage()

		{

			InitializeComponent();

			NavigationPage.SetHasNavigationBar(this, false);

			manager = ItemManager.DefaultManager;

		}

		//Se selecciona la fuente de la imagen, ya sea cámara o galería de imágenes

		async void btnImage_Clicked(object sender, EventArgs e)

		{

			MediaFile file = null;

			try

			{

				file = await ServiceImage.TakePicture(false);

			}

			catch (OperationCanceledException)

			{

				//Ignored

			}

			SetImageToControl(file);

		}

		// Se envían los resultados del cognitive services Emotions API a Azure Mobile Apps

		async void btnEnviaResultadosAzure_Clicked(object sender, EventArgs e)

		{

			if (CrossConnectivity.Current.IsConnected &&

			await CrossConnectivity.Current.IsRemoteReachable("http://www.google.com"))

			{

				Indicator.IsRunning = true;

				await manager.SaveTaskAsync(new TodoItem { Done = true, Name = ResultadoEmociones });

				await DisplayAlert("Información", "Se ha enviado tu información al server.", "Aceptar");

				Indicator.IsRunning = false;

			}

			else

			{

				await DisplayAlert("Información", "No tienes acceso a internet", "Aceptar");

			}

		}

		// Se envía la imagen a Cognitive Services para analizarla

		async void btnAnalysisEmotions_Clicked(object sender, EventArgs e)

		{

			if (CrossConnectivity.Current.IsConnected &&

			await CrossConnectivity.Current.IsRemoteReachable("http://www.google.com"))

			{

				Indicator.IsRunning = true;

				if (streamCopy != null)

				{

					Dictionary<string, float> emotions = null;

					try

					{

						streamCopy.Seek(0, SeekOrigin.Begin);

						emotions = await ServiceEmotions.GetEmotions(streamCopy);

					}

					catch (Exception ex)

					{

						Indicator.IsRunning = false;

						await

						DisplayAlert("Error", "Se ha presentado un error al conectar con los servicios", "Aceptar");

						return;

					}

					StringBuilder sb = new StringBuilder();

					if (emotions != null)

					{

						lblResult.Text = "---Análisis de Emociones---";

						DrawResults(emotions);

						foreach (var item in emotions)

						{

							string toAdd = item.Key + " : " + item.Value + " ";

							sb.Append(toAdd);

						}

					}

					else lblResult.Text = "---No se detectó una cara---";

					ResultadoEmociones += sb.ToString();

				}

				else lblResult.Text = "---No has seleccionado una imagen---";

				Indicator.IsRunning = false;

			}

			else

			{

				await DisplayAlert("Información", "No tienes acceso a internet", "Aceptar");

			}

		}

		// Se despliegan los resultados del análisis

		void DrawResults(Dictionary<string, float> emotions)

		{

			Results.Children.Clear();

			foreach (var emotion in emotions)

			{

				Label lblEmotion = new Label()

				{

					Text = emotion.Key,

					TextColor = Color.Blue,

					WidthRequest = 90

				};

				BoxView box = new BoxView()

				{

					Color = Color.Lime,

					WidthRequest = 150 * emotion.Value,

					HeightRequest = 30,

					HorizontalOptions = LayoutOptions.StartAndExpand

				};

				Label lblPercent = new Label()

				{

					Text = emotion.Value.ToString("P4"),

					TextColor = Color.Maroon

				};

				StackLayout stack = new StackLayout()

				{

					Orientation = StackOrientation.Horizontal

				};

				stack.Children.Add(lblEmotion);

				stack.Children.Add(box);

				stack.Children.Add(lblPercent);

				Results.Children.Add(stack);

			}

		}

		private async void BtnCamara_OnClicked(object sender, EventArgs e)

		{

			MediaFile file = null;

			try

			{

				file = await ServiceImage.TakePicture();

			}

			catch (OperationCanceledException)

			{

				//Ignored

			}

			SetImageToControl(file);

		}

		private void SetImageToControl(MediaFile file)

		{

			if (file == null)

			{

				return;

			}

			imgImage.Source = ImageSource.FromStream(() =>

			{

				var stream = file.GetStream();

				streamCopy = new MemoryStream();

				stream.CopyTo(streamCopy);

				stream.Seek(0, SeekOrigin.Begin);

				file.Dispose();

				return stream;

			});

		}

	}

}