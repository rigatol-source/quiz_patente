using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace quiz_patente
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont font;

        // === PLACEHOLDER GRAFICA ===
        Texture2D pixel;

        // Aree fisse (1280x720)
        Rectangle schermo = new Rectangle(0, 0, 1280, 720);
        Rectangle areaStrada = new Rectangle(440, 0, 400, 720);
        Rectangle areaQuiz = new Rectangle(0, 0, 440, 720);

        // === STRADA (movimento finto) ===
        float offsetStrada = 0f;
        float velocitaStrada = 250f;

        // === QUIZ ===
        int domandaAttuale = 0;
        int punteggio = 0;
        bool fineGioco = false;
        string esitoFinale = "";
        string messaggio = "";

        string[] domande =
        {
            "Il semaforo rosso indica:",
            "Il limite di velocità in città è:",
            "Con la pioggia la distanza di sicurezza:"
        };

        string[,] risposte =
        {
            { "Via libera", "Fermarsi", "Suonare" },
            { "50 km/h", "90 km/h", "130 km/h" },
            { "Diminuisce", "Aumenta", "Rimane uguale" }
        };

        int[] soluzioni = { 1, 0, 1 };

        KeyboardState oldKeyboard;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Font (deve esistere Font.spritefont nel Content)
            font = Content.Load<SpriteFont>("Font");

            // Pixel bianco (placeholder per disegni)
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            // QUANDO AVRAI I DISEGNI:
            // Texture2D abitacolo = Content.Load<Texture2D>("abitacolo");
            // Texture2D strada = Content.Load<Texture2D>("strada");
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.Escape))
                Exit();

            if (!fineGioco)
            {
                // Movimento strada
                offsetStrada += velocitaStrada * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (offsetStrada > 80)
                    offsetStrada = 0;

                // Input risposte
                if (KeyPressed(Keys.D1, keyboard)) Rispondi(0);
                if (KeyPressed(Keys.D2, keyboard)) Rispondi(1);
                if (KeyPressed(Keys.D3, keyboard)) Rispondi(2);
            }

            oldKeyboard = keyboard;
            base.Update(gameTime);
        }

        void Rispondi(int scelta)
        {
            if (scelta == soluzioni[domandaAttuale])
            {
                punteggio++;
                messaggio = "Risposta CORRETTA!";
            }
            else
            {
                punteggio--;
                messaggio = "Risposta SBAGLIATA!";
            }

            domandaAttuale++;

            if (domandaAttuale >= domande.Length)
                FineGioco();
        }

        void FineGioco()
        {
            fineGioco = true;
            esitoFinale = punteggio >= 2 ? "PROMOSSO!" : "BOCCIATO!";
        }

        bool KeyPressed(Keys key, KeyboardState keyboard)
        {
            return keyboard.IsKeyDown(key) && oldKeyboard.IsKeyUp(key);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGreen);

            _spriteBatch.Begin();

            // === STRADA (placeholder) ===
            _spriteBatch.Draw(pixel, areaStrada, Color.Gray);

            for (int i = 0; i < 10; i++)
            {
                Rectangle linea = new Rectangle(
                    areaStrada.X + areaStrada.Width / 2 - 5,
                    (int)(i * 80 + offsetStrada),
                    10,
                    40
                );

                _spriteBatch.Draw(pixel, linea, Color.White);
            }

            // === QUIZ ===
            if (!fineGioco)
            {
                _spriteBatch.DrawString(font, domande[domandaAttuale], new Vector2(20, 50), Color.White);

                for (int i = 0; i < 3; i++)
                {
                    _spriteBatch.DrawString(
                        font,
                        (i + 1) + ") " + risposte[domandaAttuale, i],
                        new Vector2(20, 120 + i * 40),
                        Color.White
                    );
                }

                _spriteBatch.DrawString(font, "Punteggio: " + punteggio, new Vector2(20, 300), Color.Yellow);
                _spriteBatch.DrawString(font, messaggio, new Vector2(20, 340), Color.White);
            }
            else
            {
                _spriteBatch.DrawString(font, esitoFinale, new Vector2(500, 350), Color.Red);
                _spriteBatch.DrawString(font, "Punteggio finale: " + punteggio, new Vector2(470, 400), Color.White);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
