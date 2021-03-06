/*
* AntiDupl Dynamic-Link Library.
*
* Copyright (c) 2002-2015 Yermalayeu Ihar, 2013-2015 Borisov Dmitry.
*
* Permission is hereby granted, free of charge, to any person obtaining a copy 
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
* copies of the Software, and to permit persons to whom the Software is 
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in 
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
*/
#include "adHintSetter.h"
#include "adOptions.h"
#include "adResult.h"
#include "adNeuralNetwork.h"
#include "adException.h"


namespace ad
{
    THintSetter::THintSetter(TOptions *pOptions)
        :m_pOptions(pOptions)
	    {
        //m_autoDeleteThresholdDifference = std::min(double(AUTO_DELETE_DIFFERENCE_MAX),
        //    double(m_pOptions->compare.thresholdDifference)/AUTO_DELETE_DIFFERENCE_FACTOR);
		//m_blockinessThreshold = m_pOptions->defect.blockinessThreshold;
    }

	void THintSetter::Execute(TResult *pResult, bool canRename) const
    {
		throw TException(adError::AD_ERROR_UNKNOWN);
	}

	adAlgorithmOfHintSetting THintSetter::AlgorithmOfHintSetting() const
    {
		throw TException(adError::AD_ERROR_UNKNOWN);
	}

	//-------------------------------------------------------------------------
	THintSetter_Algorithm::THintSetter_Algorithm(TOptions *pOptions)
		:THintSetter(pOptions)
    {
        m_autoDeleteThresholdDifference = std::min(double(AUTO_DELETE_DIFFERENCE_MAX),
            double(m_pOptions->compare.thresholdDifference) / AUTO_DELETE_DIFFERENCE_FACTOR);

		m_blockinessThreshold = m_pOptions->defect.blockinessThreshold;
	}

	void THintSetter_Algorithm::Execute(TResult *pResult, bool canRename) const
    {
        if(pResult->type == AD_RESULT_NONE || pResult->transform != AD_TRANSFORM_TURN_0)
        {
            pResult->hint = AD_HINT_NONE;
            return;
        }

        if(pResult->type == AD_RESULT_DEFECT_IMAGE) //if image has defect
        {
            pResult->hint = AD_HINT_DELETE_FIRST;
            return;
        }

		const TImageInfo * first = pResult->first;
		const TImageInfo * second = pResult->second;

        bool isFirstInDeletePath = 
            m_pOptions->deletePaths.IsHasPath(first->path) != AD_IS_NOT_EXIST ||
            m_pOptions->deletePaths.IsHasSubPath(first->path)  != AD_IS_NOT_EXIST;
        bool isSecondInDeletePath = 
            m_pOptions->deletePaths.IsHasPath(second->path) != AD_IS_NOT_EXIST ||
            m_pOptions->deletePaths.IsHasSubPath(second->path) != AD_IS_NOT_EXIST;

        if(pResult->difference == 0) //���� �������� �������
        {
            if(first->size > second->size) //���� ������ ������ ������ ������
            {
                if(isSecondInDeletePath || !isFirstInDeletePath) //���� ������ � ���� ��� �������� ��� ������ �� � ����
                    pResult->hint = AD_HINT_DELETE_SECOND;
                else
                    pResult->hint = canRename ? AD_HINT_RENAME_FIRST_TO_SECOND : AD_HINT_NONE;
            }
            else if(first->size < second->size)
            {
                if(!isSecondInDeletePath || isFirstInDeletePath)
                    pResult->hint = AD_HINT_DELETE_FIRST;
                else
                    pResult->hint = canRename ? AD_HINT_RENAME_SECOND_TO_FIRST : AD_HINT_NONE;
            }
            else
            {
				if(isSecondInDeletePath && !isFirstInDeletePath)
					pResult->hint = AD_HINT_DELETE_SECOND;
				else if(!isSecondInDeletePath && isFirstInDeletePath)
					pResult->hint = AD_HINT_DELETE_FIRST;
				else
				{
					if(first->time > second->time)
						pResult->hint = AD_HINT_DELETE_FIRST;
					else
						pResult->hint = AD_HINT_DELETE_SECOND;
				}
            }
            return;
        }

        if(pResult->difference < m_autoDeleteThresholdDifference && first->type == second->type) //���� �������� ������ ������ � ��� ����
        {
			if(first->size == second->size && first->Area() == second->Area() && first->blockiness < m_blockinessThreshold && second->blockiness < m_blockinessThreshold)
			{
				if(isSecondInDeletePath && !isFirstInDeletePath)
					pResult->hint = AD_HINT_DELETE_SECOND;
				else if(!isSecondInDeletePath && isFirstInDeletePath)
					pResult->hint = AD_HINT_DELETE_FIRST;
				else
				{
					if(first->time > second->time)
						pResult->hint = AD_HINT_DELETE_FIRST;
					else
						pResult->hint = AD_HINT_DELETE_SECOND;
				}
				return;
			}

            if(first->size >= second->size && first->Area() >= second->Area() && first->blockiness <= second->blockiness) //������ � ���������� ������ ������ ������
            {
                if(isSecondInDeletePath || !isFirstInDeletePath)
                    pResult->hint = AD_HINT_DELETE_SECOND;
                else
                    pResult->hint = canRename ? AD_HINT_RENAME_FIRST_TO_SECOND : AD_HINT_NONE;
                return;
            }

            if(first->size <= second->size && first->Area() <= second->Area() && first->blockiness >= second->blockiness)
            {
                if(!isSecondInDeletePath || isFirstInDeletePath)
                    pResult->hint = AD_HINT_DELETE_FIRST;
                else
                    pResult->hint = canRename ? AD_HINT_RENAME_SECOND_TO_FIRST : AD_HINT_NONE;
                return;
            }
        }

        pResult->hint = AD_HINT_NONE;
    }

	//-------------------------------------------------------------------------

	THintSetter_Neural_Network::THintSetter_Neural_Network(TOptions *pOptions)
		:THintSetter(pOptions)
		//m_pNeuralNetwork()
    {
		m_pNeuralNetwork = new TNeuralNetwork();

		//m_netLoaded = m_pNeuralNetwork->Load();
	}

	void THintSetter_Neural_Network::Execute(TResult *pResult, bool canRename) const
    {
		if(pResult->type == AD_RESULT_NONE || pResult->transform != AD_TRANSFORM_TURN_0)
        {
            pResult->hint = AD_HINT_NONE;
            return;
        }

        if(pResult->type == AD_RESULT_DEFECT_IMAGE) //if image has defect
        {
            pResult->hint = AD_HINT_DELETE_FIRST;
            return;
        }

		if (m_pNeuralNetwork->Loaded())
		{
			bool predictDelFirst = m_pNeuralNetwork->GetPredict(pResult);
			if (predictDelFirst)
				pResult->hint = AD_HINT_DELETE_FIRST;
			else
				pResult->hint = AD_HINT_DELETE_SECOND;
		}
		return;
	}

	//-------------------------------------------------------------------------
	// static
	THintSetter *THintSetterStorage::m_hintSetter_pointer; //its a definition

	THintSetter* THintSetterStorage::GetHintSetterPointer(TOptions *pOptions)
	{
		if (pOptions->hint.algorithmOfHintSetting == AD_HINT_SET_BY_ALGORITHM)
		{
			if (m_hintSetter_pointer == NULL ||
				m_hintSetter_pointer->AlgorithmOfHintSetting() != AD_HINT_SET_BY_ALGORITHM)
				m_hintSetter_pointer = new THintSetter_Algorithm(pOptions);

			return m_hintSetter_pointer;
		}
		else if (pOptions->hint.algorithmOfHintSetting == adAlgorithmOfHintSetting::AD_HINT_SET_BY_NEURAL_NETWORK)
		{
			if (m_hintSetter_pointer == NULL ||
				m_hintSetter_pointer->AlgorithmOfHintSetting() != AD_HINT_SET_BY_NEURAL_NETWORK) 
				m_hintSetter_pointer = new THintSetter_Neural_Network(pOptions);
			return m_hintSetter_pointer;
		}
		else
			throw TException(adError::AD_ERROR_INVALID_PARAMETER_COMBINATION);
	}
}