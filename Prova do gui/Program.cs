using System;
public class Produto
{
    public int Id { get; private set; }
    public string Nome { get; private set; }
    public decimal Preco { get; private set; }
    public string Categoria { get; private set; }

    public Produto(int id, string nome, decimal preco, string categoria)
    {
        if (string.IsNullOrWhiteSpace(nome)) throw new ArgumentException("Nome inválido");
        if (preco <= 0) throw new ArgumentException("Preço deve ser positivo");

        Id = id;
        Nome = nome;
        Preco = preco;
        Categoria = categoria;
    }
}
public class Cliente
{
    public int Id { get; private set; }
    public string Nome { get; private set; }
    public string Email { get; private set; }
    public string CPF { get; private set; }

    public Cliente(int id, string nome, string email, string cpf)
    {
        Id = id;
        Nome = nome;
        Email = email;
        CPF = cpf;
    }
}
public class ItemPedido
{
    public Produto Produto { get; private set; }
    public int Quantidade { get; private set; }

    public ItemPedido(Produto produto, int quantidade)
    {
        Produto = produto;
        Quantidade = quantidade;
    }

    public decimal CalcularSubtotal() => Produto.Preco * Quantidade;
}
public class Pedido
{
    public void AplicarDesconto(decimal valorDesconto)
    {
        ValorTotal -= valorDesconto;
    }

    public int Id { get; private set; }
    public Cliente Cliente { get; private set; }
    public List<ItemPedido> Itens { get; private set; }
    public DateTime Data { get; private set; }
    public decimal ValorTotal { get; private set; }

    public Pedido(int id, Cliente cliente, List<ItemPedido> itens)
    {
        Id = id;
        Cliente = cliente;
        Itens = itens;
        Data = DateTime.Now;
        ValorTotal = CalcularValorTotal();
    }
    private decimal CalcularValorTotal()
    {
        return Itens.Sum(item => item.CalcularSubtotal());
    }
}

public interface IDescontoStrategy
{
    decimal CalcularDesconto(Pedido pedido);
}
public interface ILoggerService
{
    void Log(string mensagem);
}

public interface IPedidoRepository
{
    void Adicionar(Pedido pedido);
    List<Pedido> ListarTodos();
}
public class DescontoPorCategoria : IDescontoStrategy
{
    public decimal CalcularDesconto(Pedido pedido)
    {
        var desconto = 0m;
        foreach (var item in pedido.Itens)
        {
            if (item.Produto.Categoria == "Eletrônicos")
                desconto += item.CalcularSubtotal() * 0.1m;
        }
        return desconto;
    }
}
public class DescontoPorQuantidade : IDescontoStrategy
{
    public decimal CalcularDesconto(Pedido pedido)
    {
        var desconto = 0m;
        foreach (var item in pedido.Itens)
        {
            if (item.Quantidade >= 3)
                desconto += item.CalcularSubtotal() * 0.05m;
        }
        return desconto;
    }

}
public class PedidoFactory
{
    public static Pedido CriarPedido(int id, Cliente cliente, List<ItemPedido> itens, List<IDescontoStrategy> estrategiasDeDesconto)
    {
        var pedido = new Pedido(id, cliente, itens);

        foreach (var estrategia in estrategiasDeDesconto)
        {
            var desconto = estrategia.CalcularDesconto(pedido);
            pedido.AplicarDesconto(desconto);

        }

        return pedido;
    }
}
public class PedidoRepository : IPedidoRepository
{
    private readonly List<Pedido> _pedidos = new List<Pedido>();

    public void Adicionar(Pedido pedido) => _pedidos.Add(pedido);
    public List<Pedido> ListarTodos() => _pedidos;
}
public class ConsoleLogger : ILoggerService
{
    public void Log(string mensagem)
    {
        Console.WriteLine($"LOG: {mensagem}");
    }
}
class Program
{
    static void Main()
    {
        ILoggerService logger = new ConsoleLogger();
        IPedidoRepository pedidoRepo = new PedidoRepository();

        var cliente = new Cliente(1, "Maria", "maria@email.com", "12345678900");
        var produto1 = new Produto(1, "Notebook", 3000, "Eletrônicos");
        var produto2 = new Produto(2, "Mouse", 100, "Periféricos");

        var itens = new List<ItemPedido>
        {
            new ItemPedido(produto1, 1),
            new ItemPedido(produto2, 3)
        };

        var descontos = new List<IDescontoStrategy>
        {
            new DescontoPorCategoria(),
            new DescontoPorQuantidade()
        };

        var pedido = PedidoFactory.CriarPedido(1, cliente, itens, descontos);
        pedidoRepo.Adicionar(pedido);
        logger.Log("Pedido criado com sucesso.");

        Console.WriteLine("RELATÓRIO DE PEDIDOS:");
        foreach (var p in pedidoRepo.ListarTodos())
        {
            Console.WriteLine($"Pedido #{p.Id} - Cliente: {p.Cliente.Nome} - Total: {p.ValorTotal:C}");
        }
    }

}






