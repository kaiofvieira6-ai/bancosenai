const URL_API = 'https://localhost:7081';

// HU01: Busca de Documentos por Código do Cliente
async function buscarDocumentos() {
    const codigoCliente = document.getElementById("buscaCodigoCliente").value;

    if (!codigoCliente) {
        alert("Informe o código do cliente para buscar.");
        return;
    }

    try {
        const response = await fetch(`${URL_API}/documentos/${codigoCliente}`);

        if (response.ok) {
            const documentos = await response.json();
            renderizarTabela(documentos);
        } else {
            alert("Erro ao buscar documentos ou nenhum documento encontrado.");
            renderizarTabela([]);
        }
    } catch (error) {
        console.error("Erro na busca:", error);
        alert("Erro ao conectar com o servidor.");
    }
}

// Renderiza os dados na tabela com botões de Ações (HU01, HU02, HU03)
function renderizarTabela(documentos) {
    const tbody = document.getElementById("tabelaDocumentos");
    tbody.innerHTML = "";

    documentos.forEach(doc => {
        const tr = document.createElement("tr");

        tr.innerHTML = `
            <td>${doc.id}</td>
            <td>${doc.nome}</td>
            <td>${doc.extensao}</td>
            <td>
                <button style="background-color: #ffc107; border: none; padding: 5px 10px; cursor: pointer;"
                        onclick="baixarDocumento('${doc.id}', '${doc.nome}')">
                    Baixar
                </button>
                <button style="background-color: #dc3545; color: white; border: none; padding: 5px 10px; cursor: pointer;"
                        onclick="excluirDocumento('${doc.id}')">
                    Excluir
                </button>
            </td>
        `;
        tbody.appendChild(tr);
    });
}

// HU02: Download de Arquivo
async function baixarDocumento(id, nomeArquivo) {
    try {
        const response = await fetch(`${URL_API}/download/${id}`);
        if (!response.ok) throw new Error("Falha no download");

        const blob = await response.blob();
        const urlBlob = window.URL.createObjectURL(blob);

        const a = document.createElement("a");
        a.href = urlBlob;
        a.download = nomeArquivo || "documento";
        document.body.appendChild(a);
        a.click();
        a.remove();
        window.URL.revokeObjectURL(urlBlob);
    } catch (error) {
        console.error("Erro no download:", error);
        alert("Erro ao realizar o download do arquivo.");
    }
}

// HU03: Exclusão de Arquivo
async function excluirDocumento(id) {
    if (!confirm("Deseja realmente excluir este documento?")) return;

    try {
        const response = await fetch(`${URL_API}/documentos/${id}`, {
            method: "DELETE"
        });

        if (response.ok) {
            alert("Documento excluído com sucesso!");
            // Recarrega a tabela do cliente atual
            buscarDocumentos();
        } else {
            alert("Erro ao excluir o documento.");
        }
    } catch (error) {
        console.error("Erro na exclusão:", error);
        alert("Erro ao conectar com o servidor.");
    }
}

// HU04: Upload com Atualização Automática
async function enviarDocumento() {
    const codigoClienteInput = document.getElementById("codigoCliente");
    const inputArquivo = document.getElementById("arquivo");

    const codigoCliente = codigoClienteInput.value;
    const arquivo = inputArquivo.files[0];

    if (!codigoCliente || !arquivo) {
        alert("Informe o código do cliente e selecione um arquivo");
        return;
    }

    const dadosArquivo = new FormData();
    dadosArquivo.append("arquivo", arquivo);

    try {
        const response = await fetch(`${URL_API}/upload/${codigoCliente}`, {
            method: "POST",
            body: dadosArquivo
        });

        if (response.ok) {
            alert("Documento enviado com sucesso!");

            // Sincroniza o campo de busca e dispara a atualização (HU04)
            document.getElementById("buscaCodigoCliente").value = codigoCliente;
            await buscarDocumentos();

            // Limpa o formulário de envio
            codigoClienteInput.value = "";
            inputArquivo.value = "";
        } else {
            const erro = await response.json();
            alert("Erro: " + (erro.message || "Falha ao enviar o documento"));
        }
    } catch (error) {
        console.error("Erro na requisição:", error);
        alert("Erro ao conectar com o servidor.");
    }
}