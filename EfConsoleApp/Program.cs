using EfConsoleApp.Data;
using EfConsoleApp.Models;
using Microsoft.EntityFrameworkCore;


    
        Console.Title = "EF Core Console Management System";

        // Veritabanının oluşturulduğundan emin oluyoruz
        using (var db = new AppDbContext())
        {
            db.Database.EnsureCreated();
        }

        bool running = true;
        while (running)
        {
            Console.Clear();
            DrawHeader();
            ShowMenu();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(" Seçiminiz [0-8]: ");
            Console.ResetColor();

            string input = Console.ReadLine()?.Trim() ?? "";

            switch (input)
            {
                case "1":
                    ListAllBooks();
                    break;
                case "2":
                    GetBookDetails();
                    break;
                case "3":
                    AddNewBook();
                    break;
                case "4":
                    UpdateBook();
                    break;
                case "5":
                    DeleteBook();
                    break;
                case "6":
                    SearchBooks();
                    break;
                case "7":
                    SeedSampleData();
                    break;
                case "8":
                    ShowDatabaseStats();
                    break;
                case "0":
                    running = false;
                    ShowExitScreen();
                    break;
                default:
                    ShowMessage("Geçersiz bir seçim yaptınız! Lütfen tekrar deneyin.", ConsoleColor.Red);
                    break;
            }
        }
    

    // ==========================================
    // UI BÖLÜMÜ - EKRAN BİLEŞENLERİ
    // ==========================================

     static void DrawHeader()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(@"
 ╔════════════════════════════════════════════════════════════════════════════════╗
 ║                         VERİTABANI YÖNETİM YAZILIMI                            ║
 ║                               (EF Core Panel)                                  ║
 ╚════════════════════════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
    }

     static void ShowMenu()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(" ┌──────────────────────────────────────────────────────────────────────────────┐");
        Console.ResetColor();

        PrintMenuItem("1", "Tüm Kayıtları Listele", ConsoleColor.Green);
        PrintMenuItem("2", "Kayıt Detayı Görüntüle (ID ile)", ConsoleColor.Green);
        PrintMenuItem("3", "Yeni Kayıt Ekle", ConsoleColor.Yellow);
        PrintMenuItem("4", "Mevcut Kaydı Güncelle", ConsoleColor.Yellow);
        PrintMenuItem("5", "Kayıt Sil", ConsoleColor.Red);
        PrintMenuItem("6", "Arama Yap (İsim / Yazar)", ConsoleColor.Cyan);
        PrintMenuItem("7", "Örnek Veri Yükle (Seed Data)", ConsoleColor.Magenta);
        PrintMenuItem("8", "Sistem / Veritabanı İstatistikleri", ConsoleColor.Magenta);
        PrintMenuItem("0", "Sistemden Çıkış Yap", ConsoleColor.DarkGray);

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(" └──────────────────────────────────────────────────────────────────────────────┘");
        Console.ResetColor();
        Console.WriteLine();
    }

     static void PrintMenuItem(string key, string text, ConsoleColor keyColor)
    {
        Console.Write(" │  [");
        Console.ForegroundColor = keyColor;
        Console.Write(key);
        Console.ResetColor();
        Console.WriteLine($"] {text.PadRight(68)}│");
    }

     static void ShowMessage(string message, ConsoleColor color, bool wait = true)
    {
        Console.WriteLine();
        Console.ForegroundColor = color;
        Console.WriteLine($" >>> {message}");
        Console.ResetColor();

        if (wait)
        {
            Console.WriteLine("\nDevam etmek için bir tuşa basın...");
            Console.ReadKey(true);
        }
    }

     static void RenderTable(List<Book> books)
    {
        if (books == null || !books.Any())
        {
            ShowMessage("Gösterilecek kayıt bulunamadı.", ConsoleColor.Yellow, false);
            return;
        }

        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine(" ┌─────┬──────────────────────────┬──────────────────────────┬─────────────────────┐");
        Console.WriteLine(" │ ID  │ ISIM                     │ YAZAR                    │ EKLENME TARIHI      │");
        Console.WriteLine(" ├─────┼──────────────────────────┼──────────────────────────┼─────────────────────┤");
        Console.ResetColor();

        foreach (var b in books)
        {
            string id = b.Id.ToString().PadRight(3);
            string name = Truncate(b.Name, 24).PadRight(24);
            string author = Truncate(b.AuthorName, 24).PadRight(24);
            string date = b.CreatedAt.ToString("dd.MM.yyyy HH:mm").PadRight(19);

            Console.WriteLine($" │ {id} │ {name} │ {author} │ {date} │");
        }

        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine(" └─────┴──────────────────────────┴──────────────────────────┴─────────────────────┘");
        Console.ResetColor();
        Console.WriteLine($" Toplam Kayıt Sayısı: {books.Count}");
    }

     static string Truncate(string text, int maxLength)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
        return text.Length <= maxLength ? text : text.Substring(0, maxLength - 3) + "...";
    }

    // ==========================================
    // İŞLEM BÖLÜMÜ - CRUD OPERASYONLARI
    // ==========================================

     static void ListAllBooks()
    {
        Console.Clear();
        DrawHeader();
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(" === TÜM KAYITLARIN LİSTESİ ===\n");
        Console.ResetColor();

        using (var db = new AppDbContext())
        {
            var list = db.Books.OrderByDescending(x => x.CreatedAt).ToList();
            RenderTable(list);
        }

        ShowMessage("Listeleme tamamlandı.", ConsoleColor.Gray);
    }

     static void GetBookDetails()
    {
        Console.Clear();
        DrawHeader();
        Console.WriteLine(" === KAYIT DETAYI SORGULAMA ===\n");

        Console.Write(" Detayını görmek istediğiniz Kayıt ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            ShowMessage("Geçersiz ID biçimi!", ConsoleColor.Red);
            return;
        }

        using (var db = new AppDbContext())
        {
            var item = db.Books.FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                ShowMessage($"ID: {id} olan kayıt bulunamadı!", ConsoleColor.Red);
                return;
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" ┌─────────────────────────────────────────────────────────────┐");
            Console.WriteLine($" │ ID           : {item.Id,-43} │");
            Console.WriteLine($" │ Başlık       : {Truncate(item.Name, 43),-43} │");
            Console.WriteLine($" │ Yazar        : {Truncate(item.AuthorName, 43),-43} │");
            Console.WriteLine($" │ Tarih        : {item.CreatedAt.ToString("dd.MM.yyyy HH:mm:ss"),-43} │");
            Console.WriteLine(" ├─────────────────────────────────────────────────────────────┤");
            Console.WriteLine(" │ Açıklama:                                                   │");

            string desc = item.Descripton ?? "";
            for (int i = 0; i < desc.Length; i += 55)
            {
                string chunk = desc.Substring(i, Math.Min(55, desc.Length - i));
                Console.WriteLine($" │   {chunk,-57} │");
            }

            Console.WriteLine(" └─────────────────────────────────────────────────────────────┘");
            Console.ResetColor();
        }

        ShowMessage("Sorgulama tamamlandı.", ConsoleColor.Gray);
    }

     static void AddNewBook()
    {
        Console.Clear();
        DrawHeader();
        Console.WriteLine(" === YENİ KAYIT EKLEME ===\n");

        string name = ReadRequiredInput("Kayıt/Kitap İsmi");
        string author = ReadRequiredInput("Yazar İsmi");

        Console.Write(" Açıklama (İsteğe bağlı): ");
        string desc = Console.ReadLine() ?? "";

        var newBook = new Book
        {
            Name = name,
            AuthorName = author,
            Descripton = desc,
            CreatedAt = DateTime.Now
        };

        using (var db = new AppDbContext())
        {
            db.Books.Add(newBook);
            db.SaveChanges();
        }

        ShowMessage($"Kayıt başarıyla oluşturuldu! (Atanan ID: {newBook.Id})", ConsoleColor.Green);
    }

     static void UpdateBook()
    {
        Console.Clear();
        DrawHeader();
        Console.WriteLine(" === KAYIT GÜNCELLEME ===\n");

        Console.Write(" Güncellenecek Kayıt ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            ShowMessage("Geçersiz ID!", ConsoleColor.Red);
            return;
        }

        using (var db = new AppDbContext())
        {
            var book = db.Books.FirstOrDefault(x => x.Id == id);
            if (book == null)
            {
                ShowMessage("Kayıt bulunamadı!", ConsoleColor.Red);
                return;
            }

            Console.WriteLine($"\n Mevcut İsim   : {book.Name}");
            Console.Write(" Yeni İsim (Boş bırakırsanız değişmez): ");
            string newName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newName)) book.Name = newName;

            Console.WriteLine($"\n Mevcut Yazar  : {book.AuthorName}");
            Console.Write(" Yeni Yazar (Boş bırakırsanız değişmez): ");
            string newAuthor = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newAuthor)) book.AuthorName = newAuthor;

            Console.WriteLine($"\n Mevcut Açıklama: {book.Descripton}");
            Console.Write(" Yeni Açıklama (Boş bırakırsanız değişmez): ");
            string newDesc = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newDesc)) book.Descripton = newDesc;

            db.SaveChanges();
        }

        ShowMessage("Kayıt başarıyla güncellendi!", ConsoleColor.Green);
    }

     static void DeleteBook()
    {
        Console.Clear();
        DrawHeader();
        Console.WriteLine(" === KAYIT SİLME ===\n");

        Console.Write(" Silinecek Kayıt ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            ShowMessage("Geçersiz ID!", ConsoleColor.Red);
            return;
        }

        using (var db = new AppDbContext())
        {
            var book = db.Books.FirstOrDefault(x => x.Id == id);
            if (book == null)
            {
                ShowMessage("Silinecek kayıt bulunamadı!", ConsoleColor.Red);
                return;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($" '{book.Name}' isimli kaydı silmek istediğinize emin misiniz? (E/H): ");
            Console.ResetColor();

            string confirm = Console.ReadLine()?.Trim().ToUpper();
            if (confirm == "E")
            {
                db.Books.Remove(book);
                db.SaveChanges();
                ShowMessage("Kayıt başarıyla silindi.", ConsoleColor.Green);
            }
            else
            {
                ShowMessage("Silme işlemi iptal edildi.", ConsoleColor.Yellow);
            }
        }
    }

     static void SearchBooks()
    {
        Console.Clear();
        DrawHeader();
        Console.WriteLine(" === ARAMA MOTORU ===\n");

        Console.Write(" Arama terimini girin (İsim veya Yazar): ");
        string query = Console.ReadLine()?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(query))
        {
            ShowMessage("Arama yapmak için geçerli bir metin girin!", ConsoleColor.Red);
            return;
        }

        using (var db = new AppDbContext())
        {
            var results = db.Books
                .Where(b => EF.Functions.Like(b.Name, $"%{query}%") || EF.Functions.Like(b.AuthorName, $"%{query}%"))
                .OrderByDescending(b => b.CreatedAt)
                .ToList();

            Console.WriteLine();
            RenderTable(results);
        }

        ShowMessage("Aramanız tamamlandı.", ConsoleColor.Gray);
    }

    static void SeedSampleData()
    {
        Console.Clear();
        DrawHeader();
        Console.WriteLine(" === ÖRNEK VERİ YÜKLEME (SEED DATA) ===\n");

        using (var db = new AppDbContext())
        {
            var samples = new List<Book>
                {
                    new Book { Name = "C#", AuthorName = "Ahmet", Descripton = "Clean Code.", CreatedAt = DateTime.Now.AddDays(-10) },
                    
                };

            db.Books.AddRange(samples);
            db.SaveChanges();
        }

        ShowMessage("1 adet örnek kayıt veritabanına başarıyla eklendi!", ConsoleColor.Green);
    }

     static void ShowDatabaseStats()
    {
        Console.Clear();
        DrawHeader();
        Console.WriteLine(" === İSTATİSTİKLER VE SİSTEM BİLGİSİ ===\n");

        using (var db = new AppDbContext())
        {
            int totalCount = db.Books.Count();
            var newest = db.Books.OrderByDescending(x => x.CreatedAt).FirstOrDefault();
            var oldest = db.Books.OrderBy(x => x.CreatedAt).FirstOrDefault();

            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine(" ┌─────────────────────────────────────────────────────────────┐");
            Console.WriteLine($" │ Toplam Kayıt Sayısı      : {totalCount,-32} │");
            Console.WriteLine($" │ En Son Eklenen Kayıt     : {Truncate(newest?.Name ?? "Yok", 32),-32} │");
            Console.WriteLine($" │ En Eski Kayıt            : {Truncate(oldest?.Name ?? "Yok", 32),-32} │");
            Console.WriteLine($" │ Veritabanı Sağlayıcısı   : {db.Database.ProviderName,-32} │");
            Console.WriteLine(" └─────────────────────────────────────────────────────────────┘");
            Console.ResetColor();
        }

        ShowMessage("Bilgiler güncellendi.", ConsoleColor.Gray);
    }

    // ==========================================
    // YARDIMCI METOTLAR
    // ==========================================

     static string ReadRequiredInput(string fieldName)
    {
        while (true)
        {
            Console.Write($" {fieldName}: ");
            string input = Console.ReadLine()?.Trim();

            if (!string.IsNullOrWhiteSpace(input))
                return input;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($" [!] {fieldName} boş bırakılamaz!");
            Console.ResetColor();
        }
    }

     static void ShowExitScreen()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n UYGULAMA KAPATILIYOR...\n");
        Console.ResetColor();
        Console.WriteLine(" Veritabanı bağlantıları güvenle sonlandırıldı.");
        Console.WriteLine(" İyi çalışmalar!\n");
    }

