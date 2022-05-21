using System;
using System.Diagnostics;
using System.Net;
using System.Text.Json.Nodes;

namespace UfimtsevaLounge
{
    class Program
    {
        static void Main(string[] args)
        {
            var restoraunt = new Restoraunt();
            restoraunt.run();
        }
    }

    class Restoraunt
    {
        public readonly Menu MainMenu;
        public Element ToMainMenu;
        public Dictionary<string, RestorauntItem> CartItems;
        public Element UpdateCart;

        public Restoraunt()
        {
            MainMenu = new Menu(new[]
            {
                new Element("Первые блюда") { Command = Entrees },
                new Element("Вторые блюда") { Command = Seconds },
                new Element("Гарниры") { Command = Garnishes },
                new Element("Салаты") { Command = Salads },
                new Element("Напитки") { Command = Drinks },
                new Element("\nКорзина                ") { Command = Cart },
                new Element("Оплатить и получить чек") { Command = Checkout },
                new Element("Выход                  ") { Command = Exit }
            });
            CartItems = new Dictionary<string, RestorauntItem>();
            ToMainMenu = new Element("\nВ главное меню\n") { Command = MainMenuHandler };
            UpdateCart = new Element("Обновить корзину\n") { Command = Cart };
        }

        public void run()
        {
            MainMenu.Start();
        }

        private void MainMenuHandler(Element elem, ConsoleKey key)
        {
            if (key != ConsoleKey.Enter) return;
            MainMenu.Start();
        }

        private void Entrees(Element elem, ConsoleKey key)
        {
            if (key != ConsoleKey.Enter) return;
            var entreeMenu = new Menu(new[]
            {
                new Element("Борщ", 100) { Command = CartHandler },
                new Element("Солянка", 99.5) { Command = CartHandler },
                new Element("Грибной суп", 1555.95) { Command = CartHandler },
                ToMainMenu
            });
            entreeMenu.Start();
        }

        private void Seconds(Element elem, ConsoleKey key)
        {
            if (key != ConsoleKey.Enter) return;
            var secondMenu = new Menu(new[]
            {
                new Element("Котлетка", 200) { Command = CartHandler },
                new Element("Шашлычок", 300) { Command = CartHandler },
                new Element("Отбивная", 450.23) { Command = CartHandler },
                ToMainMenu
            });
            secondMenu.Start();
        }

        private void Garnishes(Element elem, ConsoleKey key)
        {
            if (key != ConsoleKey.Enter) return;
            Menu garnishMenu = new Menu(new[]
            {
                new Element("Пюрешечка", 17) { Command = CartHandler },
                new Element("Греча", 25) { Command = CartHandler },
                new Element("Макарошки", 23) { Command = CartHandler },
                ToMainMenu
            });
            garnishMenu.Start();
        }

        private void Salads(Element elem, ConsoleKey key)
        {
            if (key != ConsoleKey.Enter) return;
            var saladMenu = new Menu(new[]
            {
                new Element("Цезарь", 250) { Command = CartHandler },
                new Element("Весенний", 345) { Command = CartHandler },
                new Element("ОЛИВЬЕХА", 567) { Command = CartHandler },
                ToMainMenu
            });
            saladMenu.Start();
        }

        private void Drinks(Element elem, ConsoleKey key)
        {
            if (key != ConsoleKey.Enter) return;
            var drinksMenu = new Menu(new[]
            {
                new Element("СосаСола", 50) { Command = CartHandler },
                new Element("Компот из черемухи", 50) { Command = CartHandler },
                new Element("стопочка", 250) { Command = CartHandler },
                ToMainMenu
            });
            drinksMenu.Start();
        }

        private void Cart(Element elem, ConsoleKey key)
        {
            if (key != ConsoleKey.Enter) return;
            var carts = new List<Element>();
            carts.Add(UpdateCart);
            foreach (var item in CartItems)
            {
                if (item.Value.Count > 0)
                {
                    carts.Add(new Element(item.Key, item.Value.Price * item.Value.Count, item.Value.Count)
                        { Command = CartHandler });
                }
            }
            carts.Add(ToMainMenu); 
            var cart = new Menu(carts.ToArray());
            cart.Start();
        }

        private void CartHandler(Element elem, ConsoleKey key)
        {
            RestorauntItem prevStateItem;
            switch (key)
            {
                case ConsoleKey.LeftArrow:
                    if (!CartItems.ContainsKey(elem.Text))
                    {
                        CartItems.Add(elem.Text, new RestorauntItem(elem.Text, 0, elem.Price));
                        break;
                    }

                    prevStateItem = CartItems[elem.Text];
                    CartItems.Remove(elem.Text);
                    if (prevStateItem.Count > 0)
                    {
                        CartItems.Add(prevStateItem.Elem,
                            new RestorauntItem(prevStateItem.Elem, prevStateItem.Count - 1, prevStateItem.Price));
                    }
                    else
                    {
                        CartItems.Add(prevStateItem.Elem,
                            new RestorauntItem(prevStateItem.Elem, 0, prevStateItem.Price));
                    }

                    break;
                case ConsoleKey.RightArrow:
                    if (!CartItems.ContainsKey(elem.Text))
                    {
                        CartItems.Add(elem.Text, new RestorauntItem(elem.Text, 1, elem.Price));
                        break;
                    }

                    prevStateItem = CartItems[elem.Text];
                    CartItems.Remove(elem.Text);
                    CartItems.Add(prevStateItem.Elem,
                        new RestorauntItem(prevStateItem.Elem, prevStateItem.Count + 1, prevStateItem.Price));

                    break;
            }
        }

        private void Checkout(Element elem, ConsoleKey key)
        {
            if (key != ConsoleKey.Enter) return;
            double total = 0;
            Console.Clear();
            Console.Write("Введите номер вашего стола: ");
            var tableNum = Console.ReadLine();
            Console.Write("Дата: ");
            Console.WriteLine(DateTime.Now);
            Console.WriteLine("\n Ваш заказ:");
            foreach (var itemArr in CartItems)
            {
                var item = itemArr.Value;
                Console.WriteLine(item.Elem + ": " + item.Count + " x " + item.Price + "р.");
                total += item.Price * item.Count;
            }
            Console.WriteLine("\n");
            Console.WriteLine("Итоговая сумма заказа: " + total + "р.");
        }

        private void Exit(Element elem, ConsoleKey key)
        {
            if (key != ConsoleKey.Enter) return;
            Environment.Exit(0);
        }
    }

    delegate void CommandHandler(Element elem, ConsoleKey key);

    class RestorauntItem
    {
        public string Elem;
        public int Count;
        public double Price;

        public RestorauntItem(string element, int count, double price)
        {
            Elem = element;
            Count = count;
            Price = price;
        }
    }


    class Menu
    {
        public Element[] Elements { get; set; }
        public int Index { get; set; }

        public Menu(Element[] elems)
        {
            Index = 0;
            Elements = elems;
            Elements[Index].IsSelected = true;
        }

        public void Start()
        {
            Index = 0;
            Elements[0].IsSelected = true;
            var isActive = true;
            while (isActive)
            {
                Draw();
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.UpArrow:
                        SelectPrev();
                        break;
                    case ConsoleKey.DownArrow:
                        SelectNext();
                        break;
                    case ConsoleKey.Enter:
                        ExecuteSelected();
                        isActive = false;
                        break;
                    case ConsoleKey.RightArrow:
                        ExecuteCartHandler(ConsoleKey.RightArrow);
                        Draw();
                        break;
                    case ConsoleKey.LeftArrow:
                        ExecuteCartHandler(ConsoleKey.LeftArrow);
                        Draw();
                        break;
                }
            }
        }

        public void Draw()
        {
            Console.Clear();
            foreach (var element in Elements)
            {
                element.Print();
            }
        }

        public void SelectNext()
        {
            if (Index == Elements.Length - 1) return;
            Elements[Index].IsSelected = false;
            Elements[++Index].IsSelected = true;
        }

        public void SelectPrev()
        {
            if (Index == 0) return;
            Elements[Index].IsSelected = false;
            Elements[--Index].IsSelected = true;
        }

        public void ExecuteSelected()
        {
            Elements[Index].Execute();
        }

        public void ExecuteCartHandler(ConsoleKey key)
        {
            Elements[Index].ExecuteCart(key);
        }
    }

    class Element
    {
        public string Text { get; set; }

        public double Price;
        public int Count;
        public ConsoleColor SelectedForeColor { get; set; }
        public ConsoleColor SelectedBackColor { get; set; }
        public bool IsSelected { get; set; }
        public CommandHandler Command;

        public Element(string text, double price = 0, int count = 0)
        {
            Text = text;
            SelectedForeColor = ConsoleColor.Black;
            SelectedBackColor = ConsoleColor.Gray;
            IsSelected = false;
            Price = price;
            Count = count;
        }

        public void Print()
        {
            if (IsSelected)
            {
                Console.BackgroundColor = SelectedBackColor;
                Console.ForegroundColor = SelectedForeColor;
            }

            var text = Text;
            if (Count > 0)
            {
                text = text + " " + Count + "шт.";
            }

            if (Price > 0)
            {
                text = text + ": " + Price + "р.";
            }
            
            Console.WriteLine(text);
            Console.ResetColor();
        }

        public void Execute()
        {
            if (Command == null) return;
            IsSelected = false;
            Command.Invoke(this, ConsoleKey.Enter);
        }

        public void ExecuteCart(ConsoleKey key)
        {
            if (Command == null) return;
            Command.Invoke(this, key);
        }
    }
}