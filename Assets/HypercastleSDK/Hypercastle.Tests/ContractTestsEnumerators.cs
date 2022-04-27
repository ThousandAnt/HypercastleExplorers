using Hypercastle.Web;
using System.Collections;
using System.Numerics;
using Unity.EditorCoroutines.Editor;
using UnityEngine;
using Assert = NUnit.Framework.Assert;

namespace Hypercastle.Tests
{
    public partial class ContractTests
    {
        IEnumerator LoadingInTheAbi()
        {
            yield return _client.LoadContract(ContractAddresses.Terraforms);
            Assert.IsNotNull(_client.TerraformsContract);
            var cachedContract = _client.TerraformsContract;
            yield return EditorCoroutineUtility.StartCoroutineOwnerless(_client.LoadContract(ContractAddresses.Terraforms));
            Assert.True(ReferenceEquals(cachedContract, _client.TerraformsContract));
        }

        IEnumerator GetMaxSupply()
        {
            yield return EditorCoroutineUtility.StartCoroutineOwnerless(
                _client.LoadContract(ContractAddresses.Terraforms));
            var task = _client.GetMaxSupply();

            while (!task.IsCompleted)
            {
                yield return null;
            }

            Debug.Log($"<color=cyan>GetMaxSupplyTest result: {task.Result.MaxSupply}</color>");
            Assert.IsNotNull(task.Result);
            Assert.True(task.Result.MaxSupply == new BigInteger(11104));
        }
        
        IEnumerator GetTokenScale()
        {
            yield return EditorCoroutineUtility.StartCoroutineOwnerless(
                _client.LoadContract(ContractAddresses.Terraforms));
            var task = _client.GetTokenScale();

            while (!task.IsCompleted)
            {
                yield return null;
            }

            Debug.Log($"<color=cyan>GetTokenScaleTest result: {task.Result.TokenScale}</color>");
            Assert.IsNotNull(task.Result);
            Assert.True(task.Result.TokenScale == new BigInteger(211808));
        }

        IEnumerator GetDreamers()
        {
            if (_client == null) Assert.Fail();
            yield return EditorCoroutineUtility.StartCoroutineOwnerless(
                _client.LoadContract(ContractAddresses.Terraforms));
            var task = _client.GetDreamers();

            while (!task.IsCompleted)
            {
                yield return null;
            }

            Debug.Log($"<color=cyan>GetDreamersTest result: {task.Result.Dreamers}</color>");
            Assert.IsNotNull(task.Result);
        }

        IEnumerator GetName()
        {
            if (_client == null) Assert.Fail();
            yield return EditorCoroutineUtility.StartCoroutineOwnerless(
                _client.LoadContract(ContractAddresses.Terraforms));
            var task = _client.GetName();
            
            while (!task.IsCompleted)
            {
                yield return null;
            }

            Debug.Log($"<color=cyan>GetDreamersTest result: {task.Result.Name}</color>");
            Assert.IsNotNull(task.Result);
        }
        
        IEnumerator GetOwner()
        {
            if (_client == null) Assert.Fail();
            yield return EditorCoroutineUtility.StartCoroutineOwnerless(
                _client.LoadContract(ContractAddresses.Terraforms));
            var task = _client.GetOwner();
            
            while (!task.IsCompleted)
            {
                yield return null;
            }

            Debug.Log($"<color=cyan>GetOwnerTest result: {task.Result.Owner}</color>");
            Assert.IsNotNull(task.Result);
        }
        
        IEnumerator GetOwnerOf()
        {
            if (_client == null) Assert.Fail();
            yield return EditorCoroutineUtility.StartCoroutineOwnerless(
                _client.LoadContract(ContractAddresses.Terraforms));
            var task = _client.GetOwnerOf(TOKEN_ID);
            
            while (!task.IsCompleted)
            {
                yield return null;
            }

            Debug.Log($"<color=cyan>GetOwnerOfTest result: {task.Result.OwnerOf}</color>");
            Assert.IsNotNull(task.Result);
        }
        
        IEnumerator GetSeed()
        {
            if (_client == null) Assert.Fail();
            yield return EditorCoroutineUtility.StartCoroutineOwnerless(
                _client.LoadContract(ContractAddresses.Terraforms));
            var task = _client.GetSeed();
            
            while (!task.IsCompleted)
            {
                yield return null;
            }

            Debug.Log($"<color=cyan>GetOwnerOfTest result: {task.Result.Seed}</color>");
            Assert.IsNotNull(task.Result);
        }
        
        IEnumerator GetStructureData()
        {
            if (_client == null) Assert.Fail();
            yield return EditorCoroutineUtility.StartCoroutineOwnerless(
                _client.LoadContract(ContractAddresses.Terraforms));
            var task = _client.GetStructureData(TIMESTAMP);
            
            while (!task.IsCompleted)
            {
                yield return null;
            }

            Debug.Log($"<color=cyan>GetStructureDataTest result:</color>\n{task.Result}");
            Assert.IsNotNull(task.Result);
        }
        
        IEnumerator GetSymbol()
        {
            if (_client == null) Assert.Fail();
            yield return EditorCoroutineUtility.StartCoroutineOwnerless(
                _client.LoadContract(ContractAddresses.Terraforms));
            var task = _client.GetSymbol();
            
            while (!task.IsCompleted)
            {
                yield return null;
            }

            Debug.Log($"<color=cyan>GetStructureDataTest result:</color>\n{task.Result.Symbol}");
            Assert.IsNotNull(task.Result);
        }
        
        IEnumerator GetTokenByIndex()
        {
            if (_client == null) Assert.Fail();
            yield return EditorCoroutineUtility.StartCoroutineOwnerless(
                _client.LoadContract(ContractAddresses.Terraforms));
            var task = _client.GetTokenByIndex(TOKEN_ID);
            
            while (!task.IsCompleted)
            {
                yield return null;
            }

            Debug.Log($"<color=cyan>GetTokenByIndexTest result:</color>\n{task.Result.TokenIndex}");
            Assert.IsNotNull(task.Result);
        }
        
        private IEnumerator GetTokenCharacters()
        {
            if (_client == null) Assert.Fail();
            yield return EditorCoroutineUtility.StartCoroutineOwnerless(
                _client.LoadContract(ContractAddresses.Terraforms));
            var task = _client.GetTokenCharacters(TOKEN_ID);
            
            while (!task.IsCompleted)
            {
                yield return null;
            }

            Debug.Log($"<color=cyan>GetOwnerOfTest result:</color>\n{task.Result}");
            Assert.IsNotNull(task.Result);
        }

        private IEnumerator GetTokenCounter()
        {
            if (_client == null) Assert.Fail();
            yield return EditorCoroutineUtility.StartCoroutineOwnerless(
                _client.LoadContract(ContractAddresses.Terraforms));
            var task = _client.GetTokenCounter();
            
            while (!task.IsCompleted)
            {
                yield return null;
            }

            Debug.Log($"<color=cyan>GetTokenCounterTest result:</color>\n{task.Result.TokenCount}");
            Assert.IsNotNull(task.Result);
        }
        
        private IEnumerator GetTokenHTML()
        {
            if (_client == null) Assert.Fail();
            yield return EditorCoroutineUtility.StartCoroutineOwnerless(
                _client.LoadContract(ContractAddresses.Terraforms));
            var task = _client.GetTokenHTML(TOKEN_ID);
            
            while (!task.IsCompleted)
            {
                yield return null;
            }

            Debug.Log($"<color=cyan>GetTokenHTMLTest result:</color>\n{task.Result.TokenHTML}");
            Assert.IsNotNull(task.Result);
        }
        
        private IEnumerator GetTokenHeightmapIndices()
        {
            if (_client == null) Assert.Fail();
            yield return EditorCoroutineUtility.StartCoroutineOwnerless(
                _client.LoadContract(ContractAddresses.Terraforms));
            var task = _client.GetTokenHeightmapIndices(TOKEN_ID);
            
            while (!task.IsCompleted)
            {
                yield return null;
            }

            Debug.Log($"<color=cyan>GetTokenHeightmapIndicesTest result:</color>\n{task.Result}");
            Assert.IsNotNull(task.Result);
        }

        IEnumerator GetTokenSVG()
        {
            if (_client == null) Assert.Fail();

            yield return EditorCoroutineUtility.StartCoroutineOwnerless(LoadingInTheAbi());
            var task = _client.GetTokenSVG(TOKEN_ID);

            while (!task.IsCompleted)
            {
                yield return null;
            }

            Debug.Log($"<color=cyan>GetSVGDataTest result:</color>\n{task.Result.TokenSVG}>");
            Assert.IsNotNull(task.Result);
        }

        IEnumerator GetTokenSupplementalData()
        {
            if (_client == null) Assert.Fail();
            yield return EditorCoroutineUtility.StartCoroutineOwnerless(
                _client.LoadContract(ContractAddresses.Terraforms));
                
            var task = _client.GetTokenSupplementalData(TOKEN_ID);

            while (!task.IsCompleted)
            {
                yield return null;
            }

            Debug.Log($"<color=cyan>GetTokenSupplementalData result:</color>\n{task.Result.tokenData}");
            Assert.IsNotNull(task.Result);
        }

        IEnumerator GetTokenTerrainValues()
        {
            if (_client == null) Assert.Fail();
            yield return EditorCoroutineUtility.StartCoroutineOwnerless(
                _client.LoadContract(ContractAddresses.Terraforms));
                
            var task = _client.GetTokenTerrainValues(TOKEN_ID);

            while (!task.IsCompleted)
            {
                yield return null;
            }

            Debug.Log($"<color=cyan>GetTokenTerrainValues result:</color>\n{task.Result}");
            Assert.IsNotNull(task.Result);
        }
        
        IEnumerator GetTokenURI()
        {
            if (_client == null) Assert.Fail();
            yield return EditorCoroutineUtility.StartCoroutineOwnerless(
                _client.LoadContract(ContractAddresses.Terraforms));
                
            var task = _client.GetTokenURI(TOKEN_ID);

            while (!task.IsCompleted)
            {
                yield return null;
            }

            Debug.Log($"<color=cyan>GetTokenTerrainValues result:</color>\n{task.Result.TokenURI}");
            Assert.IsNotNull(task.Result);
        }
        
        IEnumerator GetTotalSupply()
        {
            if (_client == null) Assert.Fail();
            yield return EditorCoroutineUtility.StartCoroutineOwnerless(
                _client.LoadContract(ContractAddresses.Terraforms));
                
            var task = _client.GetTotalSupply();

            while (!task.IsCompleted)
            {
                yield return null;
            }

            Debug.Log($"<color=cyan>GetTokenTerrainValues result:</color>\n{task.Result.TotalSupply}");
            Assert.IsNotNull(task.Result);
        }
    }
}
