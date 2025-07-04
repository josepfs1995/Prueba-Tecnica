let apiUrl = '/order';
export async function getOrder(customerName){
    let orderUrl = `${apiUrl}/get`;
    if (customerName) {
        orderUrl += `?customerName=${customerName}`;
    }
    const response = await fetch(orderUrl);
    if(!response.ok){
        throw new Error(`Error fetching order: ${response.statusText}`);
    }
    return await response.text();
}

export async function create(data){
    const response = await fetch(`${apiUrl}/create`, {
        method: 'POST',
        body: data
    });
    if(!response.ok){
        let body = await response.text();
        alert(body);
    }
    return response.ok;
}

export async function downloadExcel(){
    const response = await fetch(`${apiUrl}/DownloadExcel`);
    return { blob: await response.blob(), responseHeaders: response.headers };
}