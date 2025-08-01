﻿namespace ContatosGrupo4.Application.Configurations
{
    public class RabbitMQOptions
    {
        public required string HostName { get; set; }
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public required RabbitMQQueues Queues { get; set; }
    }

    public class RabbitMQQueues
    {
        public required string CriarContato { get; set; }
        public required string AtualizarContato { get; set; }
        public required string ExcluirContato { get; set; }
    }
}
