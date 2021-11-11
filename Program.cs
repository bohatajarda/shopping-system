using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Projekt_Bohata
{
    /*  Tento projekt slouží jako demonstrace nákupního systému. Systém načítá předem nadefinové zboží z csv souboru, v souboru se nachází 25 předmětů.
     *  Na začátku programu se zobrazí výchozí menu. Zde máme na výběr z nákupu zboží, výpis zakoupeného zboží, tisk účtenky a mazání zakoupeného zboží. 
     *  Když uživatel zadá nákup vypíše se mu seznam zboží, poté zadává počet kusů. Pokud chce ukončit nákup, může si seznam vypsat  nebo vytisknout účtenku.
     *  Poté si může účtenku vytisknout do textového souboeru.
     */
    class Program
    {
        //Deklarace globálních Listů a proměnné price, abych s nima mohl pracovat kdekoliv
        public static List<string> listItem = new List<string>();
        public static List<string> listPrice = new List<string>();
        public static List<string> listBuyedItem = new List<string>();
        public static int price = 0;
        /*  Třída Menu slouží jako výchozí bod, uživatel má zde na výběr ze 4 možností. Výběr je možno psát buď velkými písmeny nebo malými, jelikož používám převod na malá písmena.
         *  Zadávání je ošetřeno pomocí Try Catch bloků, takže pokud uživatel zadá něco jiného než text, tak se vypíše chyba a bude přesměrován do třídy Vraceni.
         */
        static void Menu()
        {
            Console.WriteLine("------------------------------------");
            Console.WriteLine("Vítej v nákupnim systému 2.0");
            Console.WriteLine("------------------------------------");
            Console.WriteLine("Nákup zboží: [N]");
            Console.WriteLine("Výpis nakoupeného zboží: [Z]");
            Console.WriteLine("Tisk účtenky: [P]");
            Console.WriteLine("Smazat zakoupené věci v seznamu zboží: [S]");
            Console.WriteLine("------------------------------------");

            try
            {
                Console.Write("Zadej písmeno pro volbu: ");
                //Deklarace a následné načítání zvolené nabídky do proměnné
                char menuChoice = char.ToLower(char.Parse(Console.ReadLine()));
                switch (menuChoice)
                {
                    case 'n':
                        Nakup();
                        break;
                    case 'z':
                        Vypis();
                        break;
                    case 'p':
                        Tisk();
                        break;
                    case 's':
                        Smazat();
                        break;
                    default:
                        Console.WriteLine("Tudy cesta nevede....");
                        Vraceni();
                        break;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Zadán chybný vstupní parametr!!!");
                Vraceni();
            }
        }
        /* Metoda Vraceni je univerzální metoda buď pro ukončení programu nebo vrácení do menu s tím, že vymaže konzoli pro lepší přehled. 
         * Tato metoda je chráněna blokem Try catch pro případné chybné zadávání hodnot. Uživatel musí zadat pouze A/a nebo N/n. Pokud zadá něco jiného podmínka to vyhodnotí
         * záporně a uživatele to přesměruje zpátky na začátek metody, kde bude znova dotazován zda chce program ukončit či nikoliv. 
         * 
         */
        static void Vraceni()
        {
            Console.WriteLine("------------------------------------");
            Console.Write("Chceš se vrátit zpět do menu [A] nebo ukončit aplikaci [N]? [A/N] ");
            try
            {
                string navrat = Console.ReadLine();
                //Převod hodnoty na velká písmena, čili je možnost zadat jak velká tak i malá písmena
                navrat = navrat.ToUpper();

                //Příkaz If vyhodnocuje zadanou hodnotu, pokud se vyhodnotí jako "A" je vymazána konzole a uživatel je přesměrován do Menu. 
                if (navrat == "A")
                {
                    Console.Clear();
                    Menu();
                }
                //Pokud se vyhodnotí jako "N" program se následně ukončí
                else if (navrat == "N")
                {
                    //Příkaz sloužící pro ukončení konzole
                    Environment.Exit(0);
                }
                //Tento příkaz zde slouží proto aby se nemohlo zadat cokoliv jiného než chceme
                else
                {
                    Console.WriteLine("Zadána špatná volba");
                    Vraceni();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Zadán chybný vstupní parametr!!!");
            }

        }

        /* Metoda Nakup slouží pro načítání souborových dat z csv souboru. Používáme tu funkci StreamReader, kvůli které je potřeba načítat knihovnu System.IO
         * Načtená data ukládáme do Listu, kdy hodnota v závorkách udává sloupec hodnot, čili načítáme pouze předměty a cenu. 
         * Data načítáme pomocí cyklu while, ten načítá data dokud nedojde ke konci záznamu. Poté hodnoty vypisujeme na konzoli.
         * Poté čekáme na uživatelův vstup, kde si vybere zboží, pokud uživatel zadá hodnotu menší než 1, tak se vypíše chyba. Podmínka zhora ošetřena není, kvůli pozdějšímu
         * přidávání zboží. Ale pokud zadáme neexistující číslo položky, tak to zachytí blok Try catch. Výpis dané položky je zajištěň pomocí indexu v listu. 
         * Nákup zboží je pomocí cyklu do while, což znamená, že uživatel je pokažde přidané položce dotázán na další pokračování nákupu. 
         */
        static void Nakup()
        {
            //listBuyedItem.Clear();
            //listItem.Clear();
            //listPrice.Clear();
            try
            {
                //Načítaní dat z csv souboru
                using (var reader = new StreamReader(@"\data.csv"))
                {
                    //Čtení csv souboru do té doby než dojde na konec souboru
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(';');
                        //Nahrazuji ";" za "|" pro lepší přehlednost
                        Console.WriteLine(line.Replace(';', '|'));
                        //Přidávání dat do listu
                        listItem.Add(values[1]);
                        listPrice.Add(values[2]);
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Nastala chyba při načítání .csv souboru!!!");
            }

            try
            {
                //Deklarace proměnných použitých pro nákup zboží
                int choice = 0;
                int multiplier = 0;
                string resume;
                int temporaryprice = 0;
                price = 0;
                //Cyklus Do While se opakuje dokud uživatel bude pokračovat v nákupu
                do
                {
                    Console.WriteLine("------------------------------------");
                    //Načítání výběru do proměnné
                    Console.Write("Pro nákup vybraného zboží napiš dané číslo: ");
                    choice = Convert.ToInt32(Console.ReadLine());
                    //Uživatel nemůže zadat položku menší než 1
                    if (choice > 0)
                    {
                        //Výpis vybraného zboží na konzoli
                        Console.WriteLine("Vybral jsi toto zboží: " + listItem[choice] + " cena: " + listPrice[choice]);
                        Console.Write("Zadej počet: ");
                        try
                        {
                            //Dotazování se na počet kusů, podmínka je zhora limitována 2000, kvůli přetečení proměnné price
                            multiplier = Convert.ToInt32(Console.ReadLine());
                            if (multiplier > 0 && multiplier < 2000)
                            {
                                //Výpočet ceny, daného zboží, která se ukládá do listu, abychom přesně věděli co uživatel nakoupil
                                temporaryprice = multiplier * int.Parse(listPrice[choice]);
                                //Přidávání nakoupeného zboží do listu
                                listBuyedItem.Add(multiplier + "x " + listItem[choice] + " cena: " + temporaryprice + " Kč");
                                price = price + (multiplier * int.Parse(listPrice[choice]));
                                Console.WriteLine("Aktuální cena zboží je: " + price);
                            }
                            else
                            {
                                Console.WriteLine("Počet kusů musí být větší než 1, ale menší než 2000");
                            }
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Zadán chybný počet kusů!!!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Výběr začíná od 1!");
                    }
                    //Dotazování se uživatele zda chce pokračovat v nákupu
                    Console.Write("Přeješ si dál nakupovat? [A/N] ");
                    resume = Console.ReadLine();
                    resume = resume.ToUpper();

                } while (resume == "A");

                //Pokud uživatel chce nákup ukončit, konzole se vymaže a uživatel je přesměrován do výchozího menu
                Console.Clear();
                Menu();


            }
            catch (Exception)
            {
                Console.WriteLine("Zadán chybný index položky!!!");
                Vraceni();
            }
        }

        //Metoda Vypis slouží pro vypisování zakoupeného zboží. Výpis je ošetřen takovým způsobem, že uživatel nic nezakoupil, čili seznam je prázdný, tak se vypíše
        //chybová hláška a uživatel je přesměrován do metody Vraceni. Pokud je seznam naplněn vypíše se pomocí cyklu ForEach. Po vypsání je uživatel přesměrován zpět do menu
        static void Vypis()
        {
            if ((listBuyedItem != null) && (!listBuyedItem.Any()))
            {
                Console.WriteLine("Seznam zakoupených věcí je prázdný, musíte nejdřív nakoupit!");
                Vraceni();
            }
            else
            {
                Console.WriteLine("Seznam zakoupeného zboží: ");
                listBuyedItem.ForEach(i => Console.WriteLine(i));
                Menu();
            }
        }

        //Definování privátní třídy random, které slouží pro vypočítávání nahodných čísel
        private static Random random = new Random();

        //Metoda RandomString slouží pro generování FIK a BKP kódu na účtenku
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        //Metoda Tisk slouží pro výpis a tisk účtenky na konzoli a případně do txt souboru
        //Výpis účtenky je ošetřenou stejnou podmínkou jako ve výpisu zboží, čili uživatel nemůže vytisknout prázdnou účtenku

        static void Tisk()
        {
            if ((listBuyedItem != null) && (!listBuyedItem.Any()))
            {
                Console.WriteLine("Nelze vytisknout prázdnou účtenku!");
                Vraceni();
            }
            else
            {
                //Deklarace proměnných potřebných na účtence a následné generování náhodných čísel
                //Výpis hodnot a dalších věcí na konzoli
                int cisloDokladu = random.Next(1000);
                int cisloProvozovna = random.Next(1, 10);
                int cisloPokladny = random.Next(1, 10);
                Console.WriteLine();
                Console.WriteLine("------------------------------------");
                Console.WriteLine("{0,25}", "Obchod u Karlose");
                Console.WriteLine("{0,27}", "Obchodní 14/34 Kamenov");
                Console.WriteLine("{0,34}", "IČO: 258963147   DIČ: CZ258963147");
                Console.WriteLine("{0,27}", "Číslo dokladu: " + cisloDokladu);
                Console.WriteLine("------------------------------------");
                Console.Write("Provozovna: " + cisloProvozovna);
                Console.WriteLine("{0,22}", "Pokladna: " + cisloPokladny);
                Console.WriteLine("Datum: " + DateTime.Now.ToString("dd/MM/yyyy"));
                Console.WriteLine("Čas: " + DateTime.Now.ToString("HH:mm:ss"));
                Console.WriteLine("------------------------------------");
                listBuyedItem.ForEach(i => Console.WriteLine(i));
                Console.WriteLine("------------------------------------");
                Console.WriteLine("{0,29}", "Celková částka: " + price + " Kč");
                Console.WriteLine("------------------------------------");
                Console.WriteLine("Režim tržby: Běžný");
                Console.WriteLine("{0,15}", "FIK");
                //Výpis hodnoty FIK, která je pevně stanovena pomocí oddělovačů
                Console.WriteLine(RandomString(8) + "-" + RandomString(4) + "-" + RandomString(4) + "-" + RandomString(4) + "-" + RandomString(12) + "-03");
                Console.WriteLine("{0,15}", "BKP");
                //Výpis hodnoty BKP která je celá náhodně generována
                Console.WriteLine(RandomString(44));

                //Dotazování se uživatele zda si chce účtenku vytisknout do textového souboru
                try
                {
                    Console.Write("Přejete si danou účtenku vytisknout do textového souboru [A/N]: ");
                    string vyberTisk = Console.ReadLine();
                    vyberTisk = vyberTisk.ToUpper();

                    if (vyberTisk == "A")
                    {
                        //Zápis do souboru funguje stejně jako výpis ze souboru, čili deklarování cesty souboru a následný zápis do souboru.
                        var cesta = (@"\uctenka.txt");
                        using var sw = new StreamWriter(cesta);
                        sw.WriteLine("------------------------------------");
                        sw.WriteLine("{0,25}", "Obchod u Karlose");
                        sw.WriteLine("{0,27}", "Obchodní 14/34 Kamenov");
                        sw.WriteLine("{0,34}", "IČO: 258963147   DIČ: CZ258963147");
                        sw.WriteLine("{0,27}", "Číslo dokladu: " + cisloDokladu);
                        sw.WriteLine("------------------------------------");
                        sw.Write("Provozovna: " + cisloProvozovna);
                        sw.WriteLine("{0,22}", "Pokladna: " + cisloPokladny);
                        sw.WriteLine("Datum: " + DateTime.Now.ToString("dd/MM/yyyy"));
                        sw.WriteLine("Čas: " + DateTime.Now.ToString("HH:mm:ss"));
                        sw.WriteLine("------------------------------------");
                        listBuyedItem.ForEach(i => sw.WriteLine(i));
                        sw.WriteLine("------------------------------------");
                        sw.WriteLine("{0,29}", "Celková částka: " + price + " Kč");
                        sw.WriteLine("------------------------------------");
                        sw.WriteLine("Režim tržby: Běžný");
                        sw.WriteLine("{0,15}", "FIK");
                        sw.WriteLine(RandomString(8) + "-" + RandomString(4) + "-" + RandomString(4) + "-" + RandomString(4) + "-" + RandomString(12) + "-03");
                        sw.WriteLine("{0,15}", "BKP");
                        sw.WriteLine(RandomString(44));
                        Console.WriteLine("Účtenka byla úspěšně vytisknuta");
                        sw.Close();
                        //Vymazání zakoupeného zboží, pokud uživatel vytiskne účtenku tak se vymaže i seznam pro lepší komfort a následné vrácení do menu
                        listBuyedItem.Clear();
                        Vraceni();
                    }
                    //Pokud uživatel nechce účtenku tisknou seznam se také vymaže a bude přesměrován do metody Vraceni
                    else
                    {
                        listBuyedItem.Clear();
                        Vraceni();
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Nastala chyba při ukládání účtenky!!");
                    Vraceni();
                }
            }
        }
        //Metoda Smazat slouží pro smazání celého seznamu zakoupéno zboží a navrácení do menu
        static void Smazat()
        {
            listBuyedItem.Clear();
            Console.WriteLine("------------------------------------");
            Console.WriteLine("Seznam byl úspěšně smazán!");
            Menu();
        }

        static void Main(string[] args)
        {
            //Prvotní volání metody Menu
            Menu();
        }
    }
}
