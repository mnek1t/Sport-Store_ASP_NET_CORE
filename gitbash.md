Клонируем содержимое удаленного репозитррия
    
    $ git clone https://gitlab.com/UserName/sports-store-application.git

Попадаем в нужный католог

    $ cd C:/RepositoryPathInFileSystem/sports-store-application

Смотрим список веток

    $ git branch -a

    * main
    sports-store-application-1
    sports-store-application-2
    sports-store-application-3
    remotes/origin/HEAD -> origin/main
    remotes/origin/main
    remotes/origin/sports-store-application-1
    remotes/origin/sports-store-application-2
    remotes/origin/sports-store-application-3
    remotes/origin/sports-store-application-4
    remotes/origin/sports-store-application-5

Переключаемся на ветку 
    $ git checkout sports-store-application-1
    
Создаем бланк сольюшена

    $ dotnet new sln --name SportsStore

Создаем проект MVC

    $ dotnet new mvc --name SportsStore

Добавляем проект в сольюшен

    $ dotnet sln add SportsStore/SportsStore.csproj

Вносим необходимые изменения в проект



    $ git add .

    $ git commit -m "FixName" 

    $ git push

    $ git checkout main

    $ git merge sports-store-application-1

Полученное в мастере изменение потом прорастить в следующую ветку для следующих изменений.
