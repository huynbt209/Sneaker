const apiEndpoint = `https://provinces.open-api.vn/api/?depth=3`;

let apiData = [];

const pOptions = document.querySelector("#p");
let dOptions = document.querySelector("#d");
let wOptions = document.querySelector("#w");
const fecthApi = async () => {
    try {
        const request = await fetch(apiEndpoint);
        const response = await request.json();
        return response;
    } catch (error) {
        console.log(error);
    }
};

const appendProvince = async () => {
    const data = await fecthApi();
    console.log(data);
    apiData = [...data];
    const firstOption = document.createElement("option");
    firstOption.disabled = true;
    firstOption.innerHTML = "Vui lòng chọn 1 tỉnh";
    firstOption.value = "default";
    pOptions.appendChild(firstOption);
    pOptions.value = "default";
    data.map((item) => {
        const option = document.createElement("option");
        option.value = item.codename;
        option.innerHTML = item.name;
        pOptions.appendChild(option);
    });
};

const handleDchange = () => {
    wOptions.remove();
    wOptions = document.createElement("select");
    wOptions.id = "w";
    const wards = document.getElementById("wards");
    wards.appendChild(wOptions);
    const queryArr = dOptions?.value.split(" ");
    const chosenProvince = apiData.filter(
        (item) => item.codename === queryArr[1],
    );
    const arrayDistricts = chosenProvince[0]?.districts;
    const choosenDistrict = arrayDistricts.filter(
        (item) => item.codename === queryArr[0],
    );
    const firstOption = document.createElement("option");
    firstOption.disabled = true;
    firstOption.innerHTML = "Vui lòng chọn 1 phường";
    firstOption.value = "default";
    wOptions.appendChild(firstOption);
    wOptions.value = "default";
    choosenDistrict[0]?.wards.map((item) => {
        const option = document.createElement("option");
        option.value = `${item.codename} ${pOptions.value}`;
        option.innerHTML = item.name;
        wOptions.appendChild(option);
    });
};

pOptions.addEventListener("change", (e) => {
    dOptions.remove();
    dOptions = document.createElement("select");
    dOptions.id = "d";
    dOptions.addEventListener("change", handleDchange);
    const districts = document.getElementById("districts");
    districts.appendChild(dOptions);

    const chosenProvince = apiData.filter(
        (item) => item.codename === pOptions.value,
    );

    if (chosenProvince[0]?.districts) {
        const { districts } = chosenProvince[0];
        const firstOption = document.createElement("option");
        firstOption.disabled = true;
        firstOption.innerHTML = "Vui lòng chọn 1 quận";
        firstOption.value = "default";
        dOptions.appendChild(firstOption);
        dOptions.value = "default";
        districts.map((item) => {
            const option = document.createElement("option");
            option.value = `${item.codename} ${pOptions.value}`;
            option.innerHTML = item.name;
            dOptions.appendChild(option);
            dOptions.classList.add("districts");
        });
    }
});

appendProvince();

