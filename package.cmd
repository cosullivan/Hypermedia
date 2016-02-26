tools\nuget.exe pack Src\Hypermedia.JsonApi.WebApi\Hypermedia.JsonApi.WebApi.nuspec -Prop Configuration=Release -BasePath Src\Hypermedia.JsonApi.WebApi\ -OutputDirectory Build\Packages

tools\nuget.exe pack Src\Hypermedia.JsonApi.Client\Hypermedia.JsonApi.Client.nuspec -Prop Configuration=Release -BasePath Src\Hypermedia.JsonApi.Client\ -OutputDirectory Build\Packages