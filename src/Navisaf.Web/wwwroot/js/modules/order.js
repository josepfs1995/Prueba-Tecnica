import { getOrder, create, downloadExcel} from "../core/order.api.js";

export async function listOrders(customerName) {
   const result = await getOrder(customerName);
   const divOrder = document.getElementById("divOrders");
   divOrder.innerHTML = result;
}
export async function searchOrder(){
   const inputCustomerName = document.getElementById("customerName");
    const customerName = inputCustomerName.value;
    await listOrders(customerName);
}
export async function createOrder(event){
    event.preventDefault();
    const form = event.target;
    const formData = new FormData(form);
    const response = await create(formData);
    if (response) {
        alert("Order created successfully!");
        form.reset();
        window.location.href = "/order";
    }
}
export async function downloadExcelReport() {
    const blobResponse = await downloadExcel();
    const url = window.URL.createObjectURL(blobResponse.blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = getFileNameFromBlob(blobResponse.responseHeaders);
    document.body.appendChild(a);
    a.click();
    a.remove();
    window.URL.revokeObjectURL(url);
}

function getFileNameFromBlob(headers) {
    const contentDisposition = headers.get('Content-Disposition');
    if (!contentDisposition) return 'downloaded_file.xlsx';

    // Caso 1: filename simple
    let match = contentDisposition.match(/filename="?(.+?)"?(;|$)/i);
    if (match && match[1]) return match[1];

    // Caso 2: filename con encoding (filename*)
    match = contentDisposition.match(/filename\*=['"]?(?:UTF-\d['"]*)?(.+?)['"]?(;|$)/i);
    if (match && match[1]) return match[1];

    // Caso 3: formato alternativo sin comillas
    match = contentDisposition.match(/filename=(.+?)(;|$)/i);
    if (match && match[1]) return match[1];

    return 'downloaded_file.xlsx';
}