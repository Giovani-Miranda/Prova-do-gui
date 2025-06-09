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


