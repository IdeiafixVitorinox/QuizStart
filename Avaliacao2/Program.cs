using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Avaliacao2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Inicializando o jogo...");
            Console.Write("Digita o teu nickname: ");
            string nickname = Console.ReadLine();
            string tema = SelecionarTema();

            Jogo jogo = new Jogo(tema, nickname);
            bool continuar = true;

            while (continuar)
            {
                Console.WriteLine("\nMenu:");
                Console.WriteLine("1. Iniciar Jogo");
                Console.WriteLine("2. Recarregar Perguntas");
                Console.WriteLine("3. Mudar Tema");
                Console.WriteLine("4. Sair");
                Console.Write("Escolhe uma opção: ");
                string escolha = Console.ReadLine();

                switch (escolha)
                {
                    case "1":
                        if (jogo.Questoes.Count == 0)
                        {
                            Console.WriteLine("Nenhuma pergunta carregada. Recarregua as perguntas primeiro.");
                        }
                        else
                        {
                            bool passouNivel = jogo.Jogar();
                            if (passouNivel)
                            {
                                if (jogo.Nivel >= 3)
                                {
                                    Console.WriteLine("\nParabéns! Ganhaste! O Jogo irá se desligar em 5 Segundos.");
                                    jogo.RegistrarHallOfFame();
                                    Thread.Sleep(5000);


                                    continuar = false;
                                }
                                else
                                {
                                    Console.WriteLine("Passaste de nível!");
                                    jogo.ProximoNivel();
                                }
                            }
                            else
                            {
                                Console.WriteLine("Perdeste!");
                                jogo.RegistrarHallOfFame();
                                jogo.ReiniciarJogo();
                            }
                        }
                        break;

                    case "2":
                        Console.WriteLine("A recarregar perguntas...");
                        jogo.CarregarPerguntas();
                        break;

                    case "3":
                        tema = SelecionarTema();
                        jogo = new Jogo(tema, nickname);
                        break;

                    case "4":
                        continuar = false;
                        break;

                    default:
                        Console.WriteLine("Opção inválida. Tenta novamente.");
                        break;
                }

                if (continuar)
                {
                    Console.WriteLine("Pressiona Enter para continuar...");
                    Console.ReadLine();
                }
            }
        }

        public static string SelecionarTema()
        {
            while (true) // Loop até que o usuário insira uma opção válida ou escolha sair
            {
                Console.WriteLine("\nTemas:");
                Console.WriteLine("1. Desporto");
                Console.WriteLine("2. Cinema");
                Console.WriteLine("3. História");
                Console.WriteLine("4. MixTemas");
                Console.WriteLine("5. Sai do jogo");
                Console.Write("\nEscolha uma opção: ");

                string escolha = Console.ReadLine();

                switch (escolha)
                {
                    case "1":
                        return "Desporto";
                    case "2":
                        return "Cinema";
                    case "3":
                        return "História";
                    case "4":
                        return "MixTemas";
                    case "5":
                        Environment.Exit(0);
                        return null;
                    default:
                        Console.WriteLine("Opção inválida. Tenta novamente.");
                        break;
                }
            }
        }
    }

    public class Questao
    {
        public string TextoQuestao { get; set; }
        public string RespostaCorreta { get; set; }
        public string[] RespostasIncorretas { get; set; }
    }

    public class Jogo
    {
        public List<Questao> Questoes { get; set; } = new List<Questao>();
        public string Tema { get; set; }
        public string Nickname { get; set; }
        public int PontuacaoTotal { get; private set; } = 0;
        public int Nivel { get; private set; } = 1;

        public Jogo(string tema, string nickname)
        {
            Tema = tema;
            Nickname = nickname;
            CarregarPerguntas();
        }

        public void CarregarPerguntas()
        {
            Questoes.Clear();
            try
            {
                string caminho = $@"{Tema}/{Tema}_Nivel{Nivel}.txt";
                if (!File.Exists(caminho))
                {
                    Console.WriteLine($"Arquivo para nível {Nivel} não encontrado. Recarregando nível 1.");
                    Nivel = 1;
                    caminho = $@"{Tema}/{Tema}_Nivel1.txt";
                }
                string[] linhas = File.ReadAllLines(caminho);
                for (int i = 0; i < linhas.Length; i++)
                {

                    while (i < linhas.Length && string.IsNullOrWhiteSpace(linhas[i]))
                    {
                        i++;
                    }

                    if (i >= linhas.Length)
                    {
                        break;
                    }

                    string textoQuestao = linhas[i].Trim();
                    i++;
                    if (i >= linhas.Length || string.IsNullOrWhiteSpace(textoQuestao))
                    {
                        continue;
                    }

                    string respostaCorretaLinha = linhas[i].Trim();
                    if (!respostaCorretaLinha.StartsWith("Resposta correta:", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    string respostaCorreta = respostaCorretaLinha.Substring("Resposta correta:".Length).Trim();
                    i++;

                    string respostasIncorretasLinha = linhas[i].Trim();
                    if (!respostasIncorretasLinha.StartsWith("Respostas erradas:", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    string[] respostasIncorretasArray = respostasIncorretasLinha.Substring("Respostas erradas:".Length).Split(',').Select(s => s.Trim()).ToArray();
                    List<string> respostasIncorretas = new List<string>(respostasIncorretasArray);

                    Questao pergunta = new Questao()
                    {
                        TextoQuestao = textoQuestao,
                        RespostaCorreta = respostaCorreta,
                        RespostasIncorretas = respostasIncorretas.ToArray()
                    };

                    Questoes.Add(pergunta);
                }

                Console.WriteLine($"\nTotal de perguntas carregadas para nível {Nivel}: {Questoes.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar perguntas: {ex.Message}");
            }
        }

        public void DefinirCorPorTema()
        {
            switch (Tema.ToLower())
            {
                case "desporto":
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case "cinema":
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case "historia":
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case "mixtemas":
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
        }


        public void ProximoNivel()
        {
            if (Nivel < 3)
            {
                Nivel++;
                CarregarPerguntas();
                Console.WriteLine("\nContinua a jogar, para o próximo nível");
            }
        }

        public bool Jogar()
        {
            int pontuacao = 0;
            Random aleatorio = new Random();

            if (Questoes.Count == 0)
            {
                Console.WriteLine("Nenhuma pergunta disponível para jogar.");
                return false;
            }

            for (int i = 0; i < 5; i++)
            {
                int questaoIndex = aleatorio.Next(Questoes.Count);
                Questao questao = Questoes[questaoIndex];

                Console.Clear();

                DefinirCorPorTema();
                Console.WriteLine(questao.TextoQuestao);

                List<string> opcoes = new List<string>(questao.RespostasIncorretas);
                opcoes.Add(questao.RespostaCorreta);
                opcoes = opcoes.OrderBy(x => aleatorio.Next()).ToList();

                Console.ForegroundColor = ConsoleColor.Green;
                for (int j = 0; j < opcoes.Count; j++)
                {
                    Console.WriteLine($"{j + 1}. {opcoes[j]}");
                }

                int resposta;
                while (!int.TryParse(Console.ReadLine(), out resposta) || resposta < 1 || resposta > opcoes.Count)
                {
                    Console.WriteLine("Por favor, insere um número entre 1 e " + opcoes.Count);
                }

                Console.ForegroundColor = ConsoleColor.Red;
                if (opcoes[resposta - 1] == questao.RespostaCorreta)
                {
                    pontuacao++;
                    Console.WriteLine("Resposta correta!");
                }
                else
                {
                    Console.WriteLine($"Resposta incorreta! A resposta correta era: {questao.RespostaCorreta}");
                }


                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"Pontuação atual: {pontuacao}");
                Console.WriteLine("Pressione Enter para continuar...");
                Console.ForegroundColor = ConsoleColor.White;
                Console.ReadLine();
            }

            Console.Clear();
            Console.WriteLine("A tua pontuação neste nível é: " + pontuacao);
            PontuacaoTotal += pontuacao;
            Console.WriteLine($"\nA tua pontuação total é de {PontuacaoTotal}");

            return pontuacao >= 4;
        }

        public void ReiniciarJogo()
        {
            Console.WriteLine("\nPerdeste no nível atual. A reiniciar o jogo...");
            Nivel = 1;
            PontuacaoTotal = 0;
            string novoTema = Program.SelecionarTema();
            Tema = novoTema;
            CarregarPerguntas();
            Console.Clear();
        }

        public void RegistrarHallOfFame()
        {
            string diretorioHallOfFame = $@"HallOfFame";
            string caminhoHallOfFame = Path.Combine(diretorioHallOfFame, "HallOfFame.txt");

            try
            {

                if (!Directory.Exists(diretorioHallOfFame))
                {
                    Directory.CreateDirectory(diretorioHallOfFame);
                }

                List<string> entradas = new List<string>();
                if (File.Exists(caminhoHallOfFame))
                {
                    entradas = File.ReadAllLines(caminhoHallOfFame).ToList();
                }

                entradas.Add($"{DateTime.Now}: {Nickname} - Pontuação total: {PontuacaoTotal} no tema {Tema}");


                entradas.Sort((x, y) =>
                {
                    int pontuacaoX = ExtrairPontuacao(x);
                    int pontuacaoY = ExtrairPontuacao(y);
                    return pontuacaoY.CompareTo(pontuacaoX);
                });


                if (entradas.Count > 20)
                {
                    entradas = entradas.Take(20).ToList();
                }


                File.WriteAllLines(caminhoHallOfFame, entradas);

                Console.WriteLine("Registro no Hall of Fame realizado com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro desconhecido ao registrar no Hall of Fame: {ex.Message}");
            }
        }

        private int ExtrairPontuacao(string entrada)
        {

            try
            {
                var partes = entrada.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var pontuacaoStr = partes[6];
                return int.Parse(pontuacaoStr);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao extrair pontuação da entrada '{entrada}': {ex.Message}");
                return 0;
            }
        }
    }
}


