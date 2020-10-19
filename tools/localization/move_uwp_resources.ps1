# List of resource folders
$input_resource_folder_list = @("src\core\Microsoft.PowerToys.Settings.UI\Strings\en-us\")
$output_resource_folder_list = @("src\core\Microsoft.PowerToys.Settings.UI\Strings\")

echo $output_resource_folder_list

# Hash table to get the folder language code from the code used in the file name
$languageHashTable = @{ "en" = "en-us";
                        "cs" = "cs-cz";
                        "de" = "de-de";
                        "es" = "es-es";
                        "fr" = "fr-fr";
                        "hu" = "hu-hu";
                        "it" = "it-it";
                        "ja" = "ja-jp";
                        "ko" = "ko-kr";
                        "nl" = "nl-nl";
                        "pl" = "pl-pl";
                        "pt-BR" = "pt-br";
                        "pt-PT" = "pt-pt";
                        "ru" = "ru-ru";
                        "sv" = "sv-se";
                        "tr" = "tr-tr";
                        "zh-Hans" =  "zh-cn";
                        "zh-Hant" = "zh-tw"
                        }

# Iterate over all folders
for ($i=0; $i -lt $input_resource_folder_list.length; $i++) {
    echo "Starting outer loop"
    Get-ChildItem "src\core\Microsoft.PowerToys.Settings.UI\Strings\"

    Get-ChildItem $input_resource_folder_list[$i] -Filter Resources.*.resw | 
    Foreach-Object {
        echo "Starting inner loop loop"
        # Get language code from file name
        $lang = "en"
        $tokens = $_.Name -split "\."
        if ($tokens.Count -eq 3) {
            $lang = $tokens[1]
        }
        $langPath = $languageHashTable[$lang]

        # Skip for en-us as it already exists in correct folder
        if ($lang -eq "en") {
            continue
        }

        # Create language folder if it doesn't exist
        $output_path = $output_resource_folder_list[$i] + $langPath
        if (!(Test-Path -Path $output_path))
        {
            $paramNewItem = @{
                Path      = $output_path
                ItemType  = 'Directory'
                Force     = $true
            }

            New-Item @paramNewItem
        }

        # UWP projects expect the file to be in the path Strings\langCode\Resources.resw where langCode is the hyphenated language code
        $input_file = $input_resource_folder_list[$i] + $_.Name
        $output_file = $output_path + "\" + "Resources.resw"

        Move-Item -Path $input_file -Destination $output_file
        echo $output_file
    }
}

