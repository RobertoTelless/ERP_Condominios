﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EntitiesServices.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ERP_CondominioEntities : DbContext
    {
        public ERP_CondominioEntities()
            : base("name=ERP_CondominioEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AGENDA> AGENDA { get; set; }
        public virtual DbSet<AGENDA_ANEXO> AGENDA_ANEXO { get; set; }
        public virtual DbSet<AGENDA_CONDOMINIO> AGENDA_CONDOMINIO { get; set; }
        public virtual DbSet<AGENDA_CONDOMINIO_ANEXO> AGENDA_CONDOMINIO_ANEXO { get; set; }
        public virtual DbSet<AGENDA_VINCULO> AGENDA_VINCULO { get; set; }
        public virtual DbSet<AMBIENTE> AMBIENTE { get; set; }
        public virtual DbSet<AMBIENTE_CHAVE> AMBIENTE_CHAVE { get; set; }
        public virtual DbSet<AMBIENTE_CUSTO> AMBIENTE_CUSTO { get; set; }
        public virtual DbSet<AMBIENTE_FINALIDADE> AMBIENTE_FINALIDADE { get; set; }
        public virtual DbSet<AMBIENTE_IMAGEM> AMBIENTE_IMAGEM { get; set; }
        public virtual DbSet<ASSINANTE> ASSINANTE { get; set; }
        public virtual DbSet<ASSINANTE_ANEXO> ASSINANTE_ANEXO { get; set; }
        public virtual DbSet<AUTORIZACAO_ACESSO> AUTORIZACAO_ACESSO { get; set; }
        public virtual DbSet<AUTORIZACAO_ACESSO_ANEXO> AUTORIZACAO_ACESSO_ANEXO { get; set; }
        public virtual DbSet<BANCO> BANCO { get; set; }
        public virtual DbSet<CARGO> CARGO { get; set; }
        public virtual DbSet<CATEGORIA_AGENDA> CATEGORIA_AGENDA { get; set; }
        public virtual DbSet<CATEGORIA_EQUIPAMENTO> CATEGORIA_EQUIPAMENTO { get; set; }
        public virtual DbSet<CATEGORIA_FORNECEDOR> CATEGORIA_FORNECEDOR { get; set; }
        public virtual DbSet<CATEGORIA_NOTIFICACAO> CATEGORIA_NOTIFICACAO { get; set; }
        public virtual DbSet<CATEGORIA_OCORRENCIA> CATEGORIA_OCORRENCIA { get; set; }
        public virtual DbSet<CATEGORIA_PRODUTO> CATEGORIA_PRODUTO { get; set; }
        public virtual DbSet<CATEGORIA_TELEFONE> CATEGORIA_TELEFONE { get; set; }
        public virtual DbSet<CATEGORIA_USUARIO> CATEGORIA_USUARIO { get; set; }
        public virtual DbSet<CENTRO_CUSTO> CENTRO_CUSTO { get; set; }
        public virtual DbSet<CONFIGURACAO> CONFIGURACAO { get; set; }
        public virtual DbSet<CONFIGURACAO_ASSOCIACAO> CONFIGURACAO_ASSOCIACAO { get; set; }
        public virtual DbSet<CONTA_BANCO> CONTA_BANCO { get; set; }
        public virtual DbSet<CONTA_BANCO_CONTATO> CONTA_BANCO_CONTATO { get; set; }
        public virtual DbSet<CONTA_BANCO_LANCAMENTO> CONTA_BANCO_LANCAMENTO { get; set; }
        public virtual DbSet<CONTA_RECEBER> CONTA_RECEBER { get; set; }
        public virtual DbSet<CONTROLE_VEICULO> CONTROLE_VEICULO { get; set; }
        public virtual DbSet<CONVIDADO> CONVIDADO { get; set; }
        public virtual DbSet<CORPO_DIRETIVO> CORPO_DIRETIVO { get; set; }
        public virtual DbSet<ENCOMENDA> ENCOMENDA { get; set; }
        public virtual DbSet<ENCOMENDA_ANEXO> ENCOMENDA_ANEXO { get; set; }
        public virtual DbSet<ENCOMENDA_COMENTARIO> ENCOMENDA_COMENTARIO { get; set; }
        public virtual DbSet<ENTRADA_SAIDA> ENTRADA_SAIDA { get; set; }
        public virtual DbSet<EQUIPAMENTO> EQUIPAMENTO { get; set; }
        public virtual DbSet<EQUIPAMENTO_ANEXO> EQUIPAMENTO_ANEXO { get; set; }
        public virtual DbSet<EQUIPAMENTO_MANUTENCAO> EQUIPAMENTO_MANUTENCAO { get; set; }
        public virtual DbSet<FINALIDADE_RESERVA> FINALIDADE_RESERVA { get; set; }
        public virtual DbSet<FORMA_ENTREGA> FORMA_ENTREGA { get; set; }
        public virtual DbSet<FORNECEDOR> FORNECEDOR { get; set; }
        public virtual DbSet<FORNECEDOR_ANEXO> FORNECEDOR_ANEXO { get; set; }
        public virtual DbSet<FORNECEDOR_COMENTARIO> FORNECEDOR_COMENTARIO { get; set; }
        public virtual DbSet<FORNECEDOR_CONTATO> FORNECEDOR_CONTATO { get; set; }
        public virtual DbSet<FORNECEDOR_MENSAGEM> FORNECEDOR_MENSAGEM { get; set; }
        public virtual DbSet<FORNECEDOR_QUADRO_SOCIETARIO> FORNECEDOR_QUADRO_SOCIETARIO { get; set; }
        public virtual DbSet<FUNCAO_CORPO_DIRETIVO> FUNCAO_CORPO_DIRETIVO { get; set; }
        public virtual DbSet<GRAU_PARENTESCO> GRAU_PARENTESCO { get; set; }
        public virtual DbSet<GRUPO> GRUPO { get; set; }
        public virtual DbSet<LISTA_CONVIDADO> LISTA_CONVIDADO { get; set; }
        public virtual DbSet<LISTA_CONVIDADO_ANEXO> LISTA_CONVIDADO_ANEXO { get; set; }
        public virtual DbSet<LISTA_CONVIDADO_COMENTARIO> LISTA_CONVIDADO_COMENTARIO { get; set; }
        public virtual DbSet<LOG> LOG { get; set; }
        public virtual DbSet<MATERIAL> MATERIAL { get; set; }
        public virtual DbSet<MATERIAL_ANEXO> MATERIAL_ANEXO { get; set; }
        public virtual DbSet<MATERIAL_FORNECEDOR> MATERIAL_FORNECEDOR { get; set; }
        public virtual DbSet<MATERIAL_MOVIMENTO> MATERIAL_MOVIMENTO { get; set; }
        public virtual DbSet<MOVIMENTO_ESTOQUE_PRODUTO> MOVIMENTO_ESTOQUE_PRODUTO { get; set; }
        public virtual DbSet<NOTICIA> NOTICIA { get; set; }
        public virtual DbSet<NOTICIA_COMENTARIO> NOTICIA_COMENTARIO { get; set; }
        public virtual DbSet<NOTIFICACAO> NOTIFICACAO { get; set; }
        public virtual DbSet<NOTIFICACAO_ANEXO> NOTIFICACAO_ANEXO { get; set; }
        public virtual DbSet<OCORRENCIA> OCORRENCIA { get; set; }
        public virtual DbSet<OCORRENCIA_ANEXO> OCORRENCIA_ANEXO { get; set; }
        public virtual DbSet<OCORRENCIA_COMENTARIO> OCORRENCIA_COMENTARIO { get; set; }
        public virtual DbSet<PERFIL> PERFIL { get; set; }
        public virtual DbSet<PERIODICIDADE> PERIODICIDADE { get; set; }
        public virtual DbSet<PERIODICIDADE_TAREFA> PERIODICIDADE_TAREFA { get; set; }
        public virtual DbSet<PRODUTO> PRODUTO { get; set; }
        public virtual DbSet<PRODUTO_ANEXO> PRODUTO_ANEXO { get; set; }
        public virtual DbSet<PRODUTO_FORNECEDOR> PRODUTO_FORNECEDOR { get; set; }
        public virtual DbSet<RESERVA> RESERVA { get; set; }
        public virtual DbSet<RESERVA_ANEXO> RESERVA_ANEXO { get; set; }
        public virtual DbSet<RESERVA_COMENTARIO> RESERVA_COMENTARIO { get; set; }
        public virtual DbSet<SOLICITACAO_MUDANCA> SOLICITACAO_MUDANCA { get; set; }
        public virtual DbSet<SOLICITACAO_MUDANCA_ANEXO> SOLICITACAO_MUDANCA_ANEXO { get; set; }
        public virtual DbSet<SOLICITACAO_MUDANCA_COMENTARIO> SOLICITACAO_MUDANCA_COMENTARIO { get; set; }
        public virtual DbSet<SOLICITACAO_MUDANCA_MOVIMENTO> SOLICITACAO_MUDANCA_MOVIMENTO { get; set; }
        public virtual DbSet<SUBCATEGORIA_PRODUTO> SUBCATEGORIA_PRODUTO { get; set; }
        public virtual DbSet<SUBGRUPO> SUBGRUPO { get; set; }
        public virtual DbSet<TAREFA> TAREFA { get; set; }
        public virtual DbSet<TAREFA_ACOMPANHAMENTO> TAREFA_ACOMPANHAMENTO { get; set; }
        public virtual DbSet<TAREFA_ANEXO> TAREFA_ANEXO { get; set; }
        public virtual DbSet<TAREFA_NOTIFICACAO> TAREFA_NOTIFICACAO { get; set; }
        public virtual DbSet<TAREFA_VINCULO> TAREFA_VINCULO { get; set; }
        public virtual DbSet<TELEFONE> TELEFONE { get; set; }
        public virtual DbSet<TEMPLATE> TEMPLATE { get; set; }
        public virtual DbSet<TIPO_AGENDA> TIPO_AGENDA { get; set; }
        public virtual DbSet<TIPO_AMBIENTE> TIPO_AMBIENTE { get; set; }
        public virtual DbSet<TIPO_CONDOMINIO> TIPO_CONDOMINIO { get; set; }
        public virtual DbSet<TIPO_CONTA> TIPO_CONTA { get; set; }
        public virtual DbSet<TIPO_DOCUMENTO> TIPO_DOCUMENTO { get; set; }
        public virtual DbSet<TIPO_ENCOMENDA> TIPO_ENCOMENDA { get; set; }
        public virtual DbSet<TIPO_GRUPO> TIPO_GRUPO { get; set; }
        public virtual DbSet<TIPO_MATERIAL> TIPO_MATERIAL { get; set; }
        public virtual DbSet<TIPO_MENSAGEM> TIPO_MENSAGEM { get; set; }
        public virtual DbSet<TIPO_PESSOA> TIPO_PESSOA { get; set; }
        public virtual DbSet<TIPO_TAREFA> TIPO_TAREFA { get; set; }
        public virtual DbSet<TIPO_UNIDADE> TIPO_UNIDADE { get; set; }
        public virtual DbSet<TIPO_VAGA> TIPO_VAGA { get; set; }
        public virtual DbSet<TIPO_VEICULO> TIPO_VEICULO { get; set; }
        public virtual DbSet<TORRE> TORRE { get; set; }
        public virtual DbSet<UF> UF { get; set; }
        public virtual DbSet<UNIDADE> UNIDADE { get; set; }
        public virtual DbSet<UNIDADE_ANEXO> UNIDADE_ANEXO { get; set; }
        public virtual DbSet<UNIDADE_MATERIAL> UNIDADE_MATERIAL { get; set; }
        public virtual DbSet<USUARIO> USUARIO { get; set; }
        public virtual DbSet<USUARIO_ANEXO> USUARIO_ANEXO { get; set; }
        public virtual DbSet<USUARIO_CONTROLE_ENTRADA> USUARIO_CONTROLE_ENTRADA { get; set; }
        public virtual DbSet<USUARIO_ESCALA_TRABALHO> USUARIO_ESCALA_TRABALHO { get; set; }
        public virtual DbSet<USUARIO_FUNCIONARIO> USUARIO_FUNCIONARIO { get; set; }
        public virtual DbSet<VAGA> VAGA { get; set; }
        public virtual DbSet<VEICULO> VEICULO { get; set; }
        public virtual DbSet<VEICULO_ANEXO> VEICULO_ANEXO { get; set; }
    }
}
