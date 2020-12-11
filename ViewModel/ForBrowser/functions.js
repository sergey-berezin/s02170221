$(_ => { })

pageNumber = 0
document.getElementById("label").innerText = pageNumber

function AddItem(name, image)
{
    document.getElementById("list").innerHTML += "<li> <div>" + name + "<div/> <img src=\"data:image/png;base64," + image + "\" width = 80 height = 80 />"
}

async function GetItems()
{
    try {
        var response = await fetch("http://localhost:5000/api/Pictures/" + pageNumber)
        var json = await response.json()

        for (let i = 0; i < json.length; i++)
        {
            AddItem("Класс: " + json[i].name + " Название картинки: " + json[i].path, json[i].dataToBase64)
        }

        if (json.length == 0)
            onClick_Previous()

        document.getElementById("label").innerText = pageNumber
    }
    catch (e)
    {
        window.alert(e)
    }
}

function onClick_Previous()
{
    if (pageNumber > 0)
    {
        pageNumber--
        document.getElementById("list").innerHTML = ""
        GetItems()
    }
}

function onClick_Next()
{
    pageNumber++
    document.getElementById("list").innerHTML = ""
    GetItems()
}

GetItems()
//AddItem("item", "https://usatiki.ru/wp-content/uploads/breeds/129/51ab9-13146922745_26fc949c5e_o-1024x869.jpg")
//AddItem("item", "https://usatiki.ru/wp-content/uploads/breeds/129/51ab9-13146922745_26fc949c5e_o-1024x869.jpg")
//AddItem("item", "https://usatiki.ru/wp-content/uploads/breeds/129/51ab9-13146922745_26fc949c5e_o-1024x869.jpg")